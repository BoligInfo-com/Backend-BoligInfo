using BoligInfo.Database;

namespace BoligInfo.Tests.Integration.Setup;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    protected BoligInfoDbContext GetDbContext()
    {
        return Factory.CreateDbContext();
    }

    protected async Task<BoligInfoDbContext> GetDbContextAsync()
    {
        return await Task.FromResult(GetDbContext());
    }

    protected async Task CleanDatabaseAsync()
    {
        using var context = GetDbContext();
        
        // Remove in correct order (children first)
        context.AllCash.RemoveRange(context.AllCash);
        context.Loans.RemoveRange(context.Loans);
        context.Equities.RemoveRange(context.Equities);
        
        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}