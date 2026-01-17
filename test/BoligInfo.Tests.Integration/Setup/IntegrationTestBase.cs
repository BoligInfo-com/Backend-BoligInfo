using BoligInfo.Database;

namespace BoligInfo.Tests.Integration.Setup;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    private readonly CustomWebApplicationFactory _factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        Client = factory.CreateClient();
    }

    private BoligInfoDbContext GetDbContext()
    {
        return _factory.CreateDbContext();
    }

    protected async Task<BoligInfoDbContext> GetDbContextAsync()
    {
        return await Task.FromResult(GetDbContext());
    }

    protected async Task CleanDatabaseAsync()
    {
        await using var context = await GetDbContextAsync();
        
        // Remove in correct order (children first, parents last)
        context.CashFlows.RemoveRange(context.CashFlows);
        context.Houses.RemoveRange(context.Houses);
        context.AllCash.RemoveRange(context.AllCash);
        context.Loans.RemoveRange(context.Loans);
        context.Equities.RemoveRange(context.Equities);
        
        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
    }
}