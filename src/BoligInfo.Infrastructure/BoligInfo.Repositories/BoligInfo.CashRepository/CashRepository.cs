using BoligInfo.Core.Models;
using BoligInfo.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Boliginfo.CashRepository;

/// <summary>
/// Repository implementation for performing CRUD operations on <see cref="Cash"/> entities.
/// </summary>
public class CashRepository(BoligInfoDbContext context, ILogger<CashRepository> logger) : ICashRepository
{
    
    // ==================== GET QUERIES ==================== //
    
    /// <summary>
    /// Retrieves all cash records from the database.
    /// </summary>
    public async Task<IEnumerable<Cash>> GetAllAsync()
    {
        logger.LogInformation("Retrieving all cash records from database, call from repository");
        
        return await context.AllCash.ToListAsync();
    }
    
    /// <summary>
    /// Retrieves a cash record by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    public async Task<Cash?> GetByIdAsync(long id)
    {
        logger.LogInformation("Retrieving cash with id: {Id} from database, call from repository", id);
        
        return await context.AllCash.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all cash records associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    public async Task<IEnumerable<Cash>> GetByEquityIdAsync(long equityId)
    {
        return await context.AllCash
            .Where(c => c.EquityId == equityId)
            .ToListAsync();
    }
    
    // ==================== POST QUERIES ==================== //
    
    /// <summary>
    /// Adds a new cash record to the database.
    /// </summary>
    /// <param name="cash">Cash entity to add.</param>
    public async Task<Cash> AddAsync(Cash cash)
    {
        logger.LogInformation("Adding new cash record for EquityId {EquityId} with amount {Amount}", cash.EquityId, cash.CashAmount);
        
        context.AllCash.Add(cash);
        await context.SaveChangesAsync();
        return cash;
    }
    
    // ==================== PUT QUERIES ==================== //
    
    /// <summary>
    /// Updates an existing cash record in the database.
    /// </summary>
    /// <param name="cash">Cash entity to update.</param>
    public async Task UpdateAsync(Cash cash)
    {
        context.AllCash.Update(cash);
        await context.SaveChangesAsync();
        
        logger.LogInformation("Updated cash record {CashId}", cash.Id);
    }
    
    // ==================== DELETE QUERIES ==================== //
    
    /// <summary>
    /// Deletes a cash record by its unique identifier.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    public async Task DeleteAsync(long id)
    {
        var cash = await context.AllCash.FindAsync(id);
        if (cash != null)
        {
            context.AllCash.Remove(cash);
            await context.SaveChangesAsync();
            
            logger.LogInformation("Deleted cash record {CashId}", id);
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
    /// Checks if a cash record with the given ID exists in the database.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    public async Task<bool> ExistsAsync(long id)
    {
        return await context.AllCash.AnyAsync(c => c.Id == id);
    }
}