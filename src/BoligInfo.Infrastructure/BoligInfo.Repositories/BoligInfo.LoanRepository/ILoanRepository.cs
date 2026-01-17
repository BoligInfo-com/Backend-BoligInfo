using BoligInfo.Core.Models;

namespace BoligInfo.LoanRepository;

/// <summary>
/// Repository interface for performing CRUD operations on <see cref="Loan"/> entities.
/// </summary>
public interface ILoanRepository
{
    /// <summary>
    /// Retrieves all loans from the database.
    /// </summary>
    Task<IEnumerable<Loan>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a loan by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    Task<Loan?> GetByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all loans associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    Task<IEnumerable<Loan>> GetByEquityIdAsync(long equityId);
    
    /// <summary>
    /// Adds a new loan to the database.
    /// </summary>
    /// <param name="loan">Loan entity to add.</param>
    Task<Loan> AddAsync(Loan loan);
    
    /// <summary>
    /// Updates an existing loan in the database.
    /// </summary>
    /// <param name="loan">Loan entity to update.</param>
    Task UpdateAsync(Loan loan);
    
    /// <summary>
    /// Deletes a loan by its unique identifier.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    Task DeleteAsync(long id);
    
    /// <summary>
    /// Checks if a loan with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    Task<bool> ExistsAsync(long id);
    
    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();
}