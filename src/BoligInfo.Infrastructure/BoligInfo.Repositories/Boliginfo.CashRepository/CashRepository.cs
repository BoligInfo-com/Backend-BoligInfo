using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace Boliginfo.CashRepository;

public class CashRepository(BoligInfoDbContext context) : ICashRepository
{
    
    // ==================== GET QUERIES ==================== //
    public async Task<IEnumerable<Cash>> GetAllAsync()
    {
        return await context.AllCash.ToListAsync();
    }
    
    public async Task<Cash?> GetByIdAsync(long id)
    {
        return await context.AllCash.FindAsync(id);
    }

    public async Task<IEnumerable<Cash>> GetByEquityIdAsync(long equityId)
    {
        return await context.AllCash
            .Where(c => c.EquityId == equityId)
            .ToListAsync();
    }
    
    // ==================== POST QUERIES ==================== //
    public async Task<Cash> AddAsync(Cash cash)
    {
        context.AllCash.Add(cash);
        await context.SaveChangesAsync();
        return cash;
    }
    
    // ==================== PUT QUERIES ==================== //
    public async Task UpdateAsync(Cash cash)
    {
        context.AllCash.Update(cash);
        await context.SaveChangesAsync();
    }
    
    // ==================== DELETE QUERIES ==================== //
    public async Task DeleteAsync(long id)
    {
        var cash = await context.AllCash.FindAsync(id);
        if (cash != null)
        {
            context.AllCash.Remove(cash);
            await context.SaveChangesAsync();
        }
    }
    
    // ==================== FUNCTIONS ==================== //
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await context.AllCash.AnyAsync(c => c.Id == id);
    }
}