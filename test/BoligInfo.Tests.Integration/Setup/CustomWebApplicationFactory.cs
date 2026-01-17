using BoligInfo.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BoligInfo.Tests.Integration.Setup;

// ReSharper disable once ClassNeverInstantiated.Global
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureServices(services =>
        {
            // Remove any existing DbContext registrations
            services.RemoveAll<DbContextOptions<BoligInfoDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<BoligInfoDbContext>();
            
            // Add in-memory database
            services.AddDbContext<BoligInfoDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
    
    public BoligInfoDbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<BoligInfoDbContext>();
    }
}