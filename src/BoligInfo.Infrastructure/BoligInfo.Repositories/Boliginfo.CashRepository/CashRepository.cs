using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace Boliginfo.CashRepository;

public class CashRepository : ICashRepository
{
    private readonly BoligInfoDbContext _context;

    public CashRepository(BoligInfoDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Cash>> GetAllAsync()
    {
        return await _context.AllCash.ToListAsync();
    }
    
    public async Task<Cash?> GetByIdAsync(long id)
    {
        return await _context.AllCash.FindAsync(id);
    }

    public async Task<IEnumerable<Cash>> GetByEquityIdAsync(long equityId)
    {
        return await _context.AllCash
            .Where(c => c.EquityId == equityId)
            .ToListAsync();
    }

    public async Task<Cash> AddAsync(Cash cash)
    {
        _context.AllCash.Add(cash);
        await _context.SaveChangesAsync();
        return cash;
    }

    public async Task UpdateAsync(Cash cash)
    {
        _context.AllCash.Update(cash);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var cash = await _context.AllCash.FindAsync(id);
        if (cash != null)
        {
            _context.AllCash.Remove(cash);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.AllCash.AnyAsync(c => c.Id == id);
    }
}