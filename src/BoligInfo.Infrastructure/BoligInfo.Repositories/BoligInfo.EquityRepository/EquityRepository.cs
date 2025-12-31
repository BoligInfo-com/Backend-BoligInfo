using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;

namespace BoligInfo.EquityRepository;


public class EquityRepository : IEquityRepository
{
    private readonly BoligInfoDbContext _context;

    public EquityRepository(BoligInfoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Equity>> GetAllAsync()
    {
        return await _context.Equities.ToListAsync();
    }

    public async Task<Equity?> GetByIdAsync(long id)
    {
        return await _context.Equities.FindAsync(id);
    }

    public async Task<Equity?> GetByIdWithLoansAsync(long id)
    {
        return await _context.Equities
            .Include(e => e.Loans)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Equity> AddAsync(Equity equity)
    {
        _context.Equities.Add(equity);
        await _context.SaveChangesAsync();
        return equity;
    }

    public async Task UpdateAsync(Equity equity)
    {
        _context.Equities.Update(equity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var equity = await _context.Equities.FindAsync(id);
        if (equity != null)
        {
            _context.Equities.Remove(equity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Equities.AnyAsync(e => e.Id == id);
    }
}