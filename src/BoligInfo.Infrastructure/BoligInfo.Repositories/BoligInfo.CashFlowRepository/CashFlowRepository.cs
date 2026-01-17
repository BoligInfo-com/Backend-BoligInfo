using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.CashFlowRepository;

public class CashFlowRepository(BoligInfoDbContext context) : ICashFlowRepository
{
    public async Task<IEnumerable<CashFlow>> GetAllAsync()
    {
        return await context.CashFlows.ToListAsync();
    }

    public async Task<CashFlow?> GetByIdAsync(long id)
    {
        return await context.CashFlows.FindAsync(id);
    }

    public async Task<IEnumerable<CashFlow>> GetByHouseIdAsync(long houseId)
    {
        return await context.CashFlows
            .Where(cf => cf.HouseId == houseId)
            .ToListAsync();
    }

    public async Task<CashFlow> AddAsync(CashFlow cashFlow)
    {
        context.CashFlows.Add(cashFlow);
        await context.SaveChangesAsync();
        return cashFlow;
    }

    public async Task UpdateAsync(CashFlow cashFlow)
    {
        context.CashFlows.Update(cashFlow);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var cashFlow = await context.CashFlows.FindAsync(id);
        if (cashFlow != null)
        {
            context.CashFlows.Remove(cashFlow);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await context.CashFlows.AnyAsync(cf => cf.Id == id);
    }
}