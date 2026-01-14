using BoligInfo.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BoligInfo.Tests.Integration.Setup;

// Base test class for setting up the test server
public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly WebApplicationFactory<Program> _factory;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BoligInfoDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<BoligInfoDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryTest_{Guid.NewGuid()}");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<BoligInfoDbContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
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