using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BoligInfo.LoanRepository;

/// <summary>
/// Repository implementation for performing CRUD operations on <see cref="Loan"/> entities.
/// </summary>
public class LoanRepository(BoligInfoDbContext context, ILogger<LoanRepository> logger) : ILoanRepository
{
    // ==================== GET QUERIES ==================== //
    
    /// <summary>
    /// Retrieves all loans from the database.
    /// </summary>
    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        logger.LogInformation("Retrieving all loans from database, call from repository");
        
        return await context.Loans.ToListAsync();
    }

    /// <summary>
    /// Retrieves a loan by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    public async Task<Loan?> GetByIdAsync(long id)
    {
        logger.LogInformation("Retrieving loan with id: {Id} from database, call from repository", id);
        
        return await context.Loans.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all loans associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    public async Task<IEnumerable<Loan>> GetByEquityIdAsync(long equityId)
    {
        return await context.Loans
            .Where(l => l.EquityId == equityId)
            .ToListAsync();
    }
    
    // ==================== POST QUERIES ==================== //
    
    /// <summary>
    /// Adds a new loan to the database.
    /// </summary>
    /// <param name="loan">Loan entity to add.</param>
    public async Task<Loan> AddAsync(Loan loan)
    {
        logger.LogInformation("Adding new loan with EquityId {EquityId} and amount {Amount}", loan.EquityId, loan.LoanAmount);
        
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return loan;
    }

    // ==================== PUT QUERIES ==================== //
    
    /// <summary>
    /// Updates an existing loan in the database.
    /// </summary>
    /// <param name="loan">Loan entity to update.</param>
    public async Task UpdateAsync(Loan loan)
    {
        context.Loans.Update(loan);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Updated loan {LoanId}", loan.Id);
    }
    
    // ==================== DELETE QUERIES ==================== //
    
    /// <summary>
    /// Deletes a loan by its unique identifier.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    public async Task DeleteAsync(long id)
    {
        var loan = await context.Loans.FindAsync(id);
        if (loan != null)
        {
            context.Loans.Remove(loan);
            await context.SaveChangesAsync();
        }
        
        logger.LogInformation("Deleted loan {LoanId}", id);
    }

    // ==================== FUNCTIONS ==================== //
    
    /// <summary>
    /// Checks if a loan with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    public async Task<bool> ExistsAsync(long id)
    {
        return await context.Loans.AnyAsync(l => l.Id == id);
    }

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}