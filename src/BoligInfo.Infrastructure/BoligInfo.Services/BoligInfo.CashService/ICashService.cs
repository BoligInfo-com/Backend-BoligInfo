using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;

namespace BoligInfo.CashService;

/// <summary>
/// Service interface for managing <see cref="Cash"/> entities.
/// Handles business logic, validation, and DTO mapping.
/// </summary>
public interface ICashService
{
    /// <summary>
    /// Retrieves all cash records.
    /// </summary>
    Task<IEnumerable<CashDto>> GetAllCashAsync();
    
    /// <summary>
    /// Retrieves a cash record by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    Task<CashDto?> GetCashByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all cash records associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    Task<IEnumerable<CashDto>> GetAllCashByEquityIdAsync(long equityId);
    
    /// <summary>
    /// Creates a new cash record.
    /// Throws <see cref="KeyNotFoundException"/> if the parent equity does not exist.
    /// Throws <see cref="InvalidOperationException"/> if the equity already has cash.
    /// </summary>
    /// <param name="createCashDto">Data for creating cash.</param>
    Task<CashDto> CreateCashAsync(CreateCashDto createCashDto);
    
    /// <summary>
    /// Updates an existing cash record.
    /// Throws <see cref="KeyNotFoundException"/> if the cash record does not exist.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    /// <param name="updateCashDto">Data for updating cash.</param>
    Task<CashDto> UpdateCashAsync(long id, UpdateCashDto updateCashDto);
    
    /// <summary>
    /// Deletes a cash record by its ID.
    /// Throws <see cref="KeyNotFoundException"/> if the cash record does not exist.
    /// </summary>
    /// <param name="id">Cash ID.</param>
    Task DeleteCashAsync(long id);
}