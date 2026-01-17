using BoligInfo.Core.Models;

namespace Boliginfo.CashRepository;

/// <summary>
/// Repository interface for performing CRUD operations on <see cref="Cash"/> entities.
/// </summary>
public interface ICashRepository
{
    /// <summary>
    /// Retrieves all cash records from the database.
    /// </summary>
    Task<IEnumerable<Cash>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a cash record by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    Task<Cash?> GetByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all cash records associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    Task<IEnumerable<Cash>> GetByEquityIdAsync(long equityId);
   
    /// <summary>
    /// Adds a new cash record to the database.
    /// </summary>
    /// <param name="cash">Cash entity to add.</param>
    Task<Cash> AddAsync(Cash cash);
    
    /// <summary>
    /// Updates an existing cash record in the database.
    /// </summary>
    /// <param name="cash">Cash entity to update.</param>
    Task UpdateAsync(Cash cash);
    
    /// <summary>
    /// Deletes a cash record by its unique identifier.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    Task DeleteAsync(long id);
    
    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task SaveChangesAsync();
    
    /// <summary>
    /// Checks if a cash record with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    Task<bool> ExistsAsync(long id);
}