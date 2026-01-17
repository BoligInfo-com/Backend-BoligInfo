using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;


namespace BoligInfo.LoanRepository;

public class LoanRepository(BoligInfoDbContext context) : ILoanRepository
{
    // ==================== GET QUERIES ==================== //
    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        return await context.Loans.ToListAsync();
    }

    public async Task<Loan?> GetByIdAsync(long id)
    {
        return await context.Loans.FindAsync(id);
    }

    public async Task<IEnumerable<Loan>> GetByEquityIdAsync(long equityId)
    {
        return await context.Loans
            .Where(l => l.EquityId == equityId)
            .ToListAsync();
    }
    
    // ==================== POST QUERIES ==================== //
    public async Task<Loan> AddAsync(Loan loan)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return loan;
    }

    // ==================== PUT QUERIES ==================== //
    public async Task UpdateAsync(Loan loan)
    {
        context.Loans.Update(loan);
        await context.SaveChangesAsync();
    }
    
    // ==================== DELETE QUERIES ==================== //
    public async Task DeleteAsync(long id)
    {
        var loan = await context.Loans.FindAsync(id);
        if (loan != null)
        {
            context.Loans.Remove(loan);
            await context.SaveChangesAsync();
        }
    }

    // ==================== FUNCTIONS ==================== //
    public async Task<bool> ExistsAsync(long id)
    {
        return await context.Loans.AnyAsync(l => l.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}