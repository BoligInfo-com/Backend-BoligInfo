using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BoligInfo.EquityRepository;


/// <summary>
/// Repository implementation for performing CRUD operations on <see cref="Equity"/> entities.
/// </summary>
public class EquityRepository(BoligInfoDbContext context, ILogger<EquityRepository> logger) : IEquityRepository
{
    // ==================== GET QUERIES ==================== //
    
    /// <summary>
    /// Retrieves all equity records from the database.
    /// </summary>
    public async Task<IEnumerable<Equity>> GetAllAsync()
    {
        logger.LogInformation("Retrieving all equity records, call from repository");
        return await context.Equities.ToListAsync();
    }

    /// <summary>
    /// Retrieves an equity record by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    public async Task<Equity?> GetByIdAsync(long id)
    {
        logger.LogInformation("Retrieving equity with id: {Id} from database, call from repository", id);
        return await context.Equities.FindAsync(id);
    }
    
    /// <summary>
    /// Retrieves an equity record including its associated loans.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    public async Task<Equity?> GetByIdWithLoansAsync(long id)
    {
        return await context.Equities
            .Include(e => e.Loans)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    
    /// <summary>
    /// Retrieves an equity record including its associated cash.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    public async Task<Equity?> GetByIdWithCashAsync(long id)
    {
        return await context.Equities
            .Include(e => e.Cash)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // ==================== POST QUERIES ==================== //
    
    /// <summary>
    /// Adds a new equity record to the database.
    /// </summary>
    /// <param name="equity">Equity entity to add.</param>
    public async Task<Equity> AddAsync(Equity equity)
    {
        logger.LogInformation("Added new equity with ID {EquityId}", equity.Id);
        
        context.Equities.Add(equity);
        await context.SaveChangesAsync();
        return equity;
    }

    // ==================== PUT QUERIES ==================== //
    
    /// <summary>
    /// Updates an existing equity record in the database.
    /// </summary>
    /// <param name="equity">Equity entity to update.</param>
    public async Task UpdateAsync(Equity equity)
    {
        context.Equities.Update(equity);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Updated equity record {EquityId}", equity.Id);
    }

    // ==================== DELETE QUERIES ==================== //
    
    /// <summary>
    /// Deletes an equity record by its unique identifier.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    public async Task DeleteAsync(long id)
    {
        var equity = await context.Equities.FindAsync(id);
        if (equity != null)
        {
            context.Equities.Remove(equity);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Deleted equity record {EquityId}", id);
        }
    }
    
    // ==================== FUNCTIONS ==================== //
    
    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if an equity record with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    public async Task<bool> ExistsAsync(long id)
    {
        return await context.Equities.AnyAsync(e => e.Id == id);
    }
}