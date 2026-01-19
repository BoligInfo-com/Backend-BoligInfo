using BoligInfo.Core.Models;

namespace BoligInfo.HouseRepository;

/// <summary>
/// Defines data access operations for <see cref="House"/> entities.
/// </summary>
public interface IHouseRepository
{
    /// <summary>
    /// Retrieves all houses.
    /// </summary>
    Task<IEnumerable<House>> GetAllAsync();
    
    /// <summary>
    /// Retrieves a house by its unique identifier.
    /// </summary>
    /// <param name="id">The house ID.</param>
    Task<House?> GetByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all houses associated with a specific equity.
    /// </summary>
    /// <param name="equityId">The equity ID.</param>
    Task<IEnumerable<House>> GetByEquityIdAsync(long equityId);
    
    /// <summary>
    /// Retrieves a house by ID including related cash flows.
    /// </summary>
    /// <param name="id">The house ID.</param>
    Task<House?> GetByIdWithCashFlowsAsync(long id);
    
    /// <summary>
    /// Adds a new house to the database.
    /// </summary>
    /// <param name="house">The house entity to add.</param>
    Task<House> AddAsync(House house);
    
    /// <summary>
    /// Updates an existing house.
    /// </summary>
    /// <param name="house">The house entity with updated values.</param>
    Task UpdateAsync(House house);
    
    /// <summary>
    /// Deletes a house by its ID.
    /// </summary>
    /// <param name="id">The house ID.</param>
    Task DeleteAsync(long id);
    
    /// <summary>
    /// Checks whether a house exists.
    /// </summary>
    /// <param name="id">The house ID.</param>
    Task<bool> ExistsAsync(long id);
}