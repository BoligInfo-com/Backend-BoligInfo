using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.EquityRepository;


public class EquityRepository(BoligInfoDbContext context) : IEquityRepository
{
    // ==================== GET QUERIES ==================== //
    public async Task<IEnumerable<Equity>> GetAllAsync()
    {
        return await context.Equities.ToListAsync();
    }

    public async Task<Equity?> GetByIdAsync(long id)
    {
        return await context.Equities.FindAsync(id);
    }

    public async Task<Equity?> GetByIdWithLoansAsync(long id)
    {
        return await context.Equities
            .Include(e => e.Loans)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<Equity?> GetByIdWithCashAsync(long id)
    {
        return await context.Equities
            .Include(e => e.Cash)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // ==================== POST QUERIES ==================== //
    public async Task<Equity> AddAsync(Equity equity)
    {
        context.Equities.Add(equity);
        await context.SaveChangesAsync();
        return equity;
    }

    // ==================== PUT QUERIES ==================== //
    public async Task UpdateAsync(Equity equity)
    {
        context.Equities.Update(equity);
        await context.SaveChangesAsync();
    }

    // ==================== DELETE QUERIES ==================== //
    public async Task DeleteAsync(long id)
    {
        var equity = await context.Equities.FindAsync(id);
        if (equity != null)
        {
            context.Equities.Remove(equity);
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
        return await context.Equities.AnyAsync(e => e.Id == id);
    }
}