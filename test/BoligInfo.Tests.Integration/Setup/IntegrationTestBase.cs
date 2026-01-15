using BoligInfo.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BoligInfo.Tests.Integration.Setup;

public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    private readonly WebApplicationFactory<Program> _factory;

    protected IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Set environment to "Test" so Program.cs skips PostgresSQL setup
            builder.UseEnvironment("Test");
            
            builder.ConfigureServices(services =>
            {
                // Remove any existing DbContext registrations
                services.RemoveAll<DbContextOptions<BoligInfoDbContext>>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<BoligInfoDbContext>();
                
                // Add in-memory database for testing
                services.AddDbContext<BoligInfoDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryTest_{Guid.NewGuid()}");
                });

                // Build service provider and ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BoligInfoDbContext>();
                db.Database.EnsureCreated();
            });
        });

        Client = _factory.CreateClient();
    }

    protected async Task<BoligInfoDbContext> GetDbContextAsync()
    {
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BoligInfoDbContext>();
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    protected async Task CleanDatabaseAsync()
    {
        var context = await GetDbContextAsync();
        context.AllCash.RemoveRange(context.AllCash);
        context.Loans.RemoveRange(context.Loans);
        context.Equities.RemoveRange(context.Equities);
        await context.SaveChangesAsync();
    }
}