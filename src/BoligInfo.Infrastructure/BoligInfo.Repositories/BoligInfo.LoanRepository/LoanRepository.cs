using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;


namespace BoligInfo.LoanRepository;

public class LoanRepository : ILoanRepository
{
    private readonly BoligInfoDbContext _context;

    public LoanRepository(BoligInfoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await _context.Loans.ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(long id)
    {
        return await _context.Loans.FindAsync(id);
    }

    public async Task<IEnumerable<Loan>> GetByEquityIdAsync(long equityId)
    {
        return await _context.Loans
            .Where(l => l.EquityId == equityId)
            .ToListAsync();
    }

    public async Task<Loan> AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var loan = await _context.Loans.FindAsync(id);
        if (loan != null)
        {
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.Loans.AnyAsync(l => l.Id == id);
    }
}