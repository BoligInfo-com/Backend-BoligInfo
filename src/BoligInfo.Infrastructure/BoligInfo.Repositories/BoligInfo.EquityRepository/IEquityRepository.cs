using BoligInfo.Core.Models;

namespace BoligInfo.EquityRepository;

/// <summary>
/// Repository interface for performing CRUD operations on <see cref="Equity"/> entities.
/// </summary>
public interface IEquityRepository
{
    /// <summary>
    /// Retrieves all equity records from the database.
    /// </summary>
    Task<IEnumerable<Equity>> GetAllAsync();
    
    /// <summary>
    /// Retrieves an equity record by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<Equity?> GetByIdAsync(long id);
    
    /// <summary>
    /// Retrieves an equity record including its associated loans.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<Equity?> GetByIdWithLoansAsync(long id);
    
    /// <summary>
    /// Retrieves an equity record including its associated cash.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<Equity?> GetByIdWithCashAsync(long id);
    
    /// <summary>
    /// Adds a new equity record to the database.
    /// </summary>
    /// <param name="equity">Equity entity to add.</param>
    Task<Equity> AddAsync(Equity equity);
    
    /// <summary>
    /// Updates an existing equity record in the database.
    /// </summary>
    /// <param name="equity">Equity entity to update.</param>
    Task UpdateAsync(Equity equity);
    
    /// <summary>
    /// Deletes an equity record by its unique identifier.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task DeleteAsync(long id);
    
    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();
    
    /// <summary>
    /// Checks if an equity record with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<bool> ExistsAsync(long id);
}