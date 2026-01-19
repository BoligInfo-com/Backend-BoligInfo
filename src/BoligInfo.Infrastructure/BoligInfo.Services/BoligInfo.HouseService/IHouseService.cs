using BoligInfo.Core.DTO;

namespace BoligInfo.HouseService;

/// <summary>
/// Defines business logic operations for managing houses.
/// </summary>
public interface IHouseService
{
    /// <summary>
    /// Retrieves all houses.
    /// </summary>
    /// <returns>A collection of houses.</returns>
    Task<IEnumerable<HouseDto>> GetAllHousesAsync();
    
    /// <summary>
    /// Retrieves a house by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the house.</param>
    /// <returns>The house if found; otherwise null.</returns>
    Task<HouseDto?> GetHouseByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all houses associated with a specific equity.
    /// </summary>
    /// <param name="equityId">The equity ID.</param>
    /// <returns>A collection of houses.</returns>
    Task<IEnumerable<HouseDto>> GetHousesByEquityIdAsync(long equityId);
    
    /// <summary>
    /// Retrieves a house including its associated cash flows.
    /// </summary>
    /// <param name="id">The house ID.</param>
    /// <returns>The house with cash flows if found; otherwise null.</returns>
    Task<HouseDto?> GetHouseWithCashFlowsAsync(long id);
    
    /// <summary>
    /// Creates a new house.
    /// </summary>
    /// <param name="createHouseDto">The house creation data.</param>
    /// <returns>The created house.</returns>
    Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto);
    
    /// <summary>
    /// Updates an existing house.
    /// </summary>
    /// <param name="id">The house ID.</param>
    /// <param name="updateHouseDto">The update data.</param>
    /// <returns>The updated house.</returns>
    Task<HouseDto> UpdateHouseAsync(long id, UpdateHouseDto updateHouseDto);
    
    /// <summary>
    /// Deletes a house and its related data.
    /// </summary>
    /// <param name="id">The house ID.</param>
    Task DeleteHouseAsync(long id);
}