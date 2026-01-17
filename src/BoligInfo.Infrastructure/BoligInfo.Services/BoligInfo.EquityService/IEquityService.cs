using BoligInfo.Core.DTO;

namespace BoligInfo.EquityService;

/// <summary>
/// Service interface for managing <see cref="Equity"/> entities.
/// Handles business logic, validation, and DTO mapping.
/// </summary>
public interface IEquityService
{
    /// <summary>
    /// Retrieves all equities.
    /// </summary>
    Task<IEnumerable<EquityDto>> GetAllEquitiesAsync();
    
    /// <summary>
    /// Retrieves a single equity by ID.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<EquityDto?> GetEquityByIdAsync(long id);
    
    /// <summary>
    /// Retrieves a single equity with all associated loans.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<EquityDto?> GetEquityWithLoansAsync(long id);
    
    /// <summary>
    /// Retrieves a single equity with its associated cash.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task<EquityDto?> GetEquityWithCashAsync(long id);
    
    /// <summary>
    /// Creates a new equity.
    /// </summary>
    /// <param name="createEquityDto">Data for creating equity.</param>
    Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto);
    
    /// <summary>
    /// Updates an existing equity.
    /// Throws <see cref="KeyNotFoundException"/> if equity does not exist.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    /// <param name="updateEquityDto">Data for updating equity.</param>
    Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto);
    
    /// <summary>
    /// Deletes an equity and its associated loans and cash.
    /// Throws <see cref="KeyNotFoundException"/> if equity does not exist.
    /// </summary>
    /// <param name="id">Equity ID.</param>
    Task DeleteEquityAsync(long id);
}