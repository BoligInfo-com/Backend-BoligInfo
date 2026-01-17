using Boliginfo.CashRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using Microsoft.Extensions.Logging;

namespace BoligInfo.CashService;


/// <summary>
/// Service for managing <see cref="Cash"/> entities.
/// Handles validation, business logic, and DTO mapping.
/// </summary>
public class CashService(
    ICashRepository cashRepository, 
    IEquityRepository equityRepository,
    ILogger<CashService> logger) : ICashService
{
    // ==================== GET VALIDATION ==================== //
    
    /// <summary>
    /// Retrieves all cash records and maps them to DTOs.
    /// </summary>
    public async Task<IEnumerable<CashDto>> GetAllCashAsync()
    {
        var allCash = await cashRepository.GetAllAsync();

        var enumerable = allCash.ToList();
        logger.LogInformation("Retrieved {Count} cash records", enumerable.Count);
        
        return enumerable.Select(MapToDto);
    }
    
    /// <summary>
    /// Retrieves a cash record by ID and maps it to a DTO.
    /// Returns null if not found.
    /// </summary>
    public async Task<CashDto?> GetCashByIdAsync(long id)
    {
        var cash = await cashRepository.GetByIdAsync(id);
        
        if (cash == null) logger.LogWarning("Cash {CashId} not found", id);
        
        return cash == null ? null : MapToDto(cash);
    }

    /// <summary>
    /// Retrieves all cash records for a specific equity.
    /// </summary>
    public async Task<IEnumerable<CashDto>> GetAllCashByEquityIdAsync(long equityId)
    {
        var allCash = await cashRepository.GetByEquityIdAsync(equityId);
        return allCash.Select(MapToDto); 
    }

    // ==================== POST VALIDATION ==================== //
    
    /// <summary>
    /// Creates a new cash record after validating parent equity and one-to-one constraint.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if equity does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if equity already has cash.</exception>
    public async Task<CashDto> CreateCashAsync(CreateCashDto createCashDto)
    {
        // Ensure parent Equity exists
        var equityExists  = await equityRepository.ExistsAsync(createCashDto.EquityId);
        if (!equityExists)
        {
            logger.LogWarning("Failed to create cash: Equity {EquityId} not found", createCashDto.EquityId);
            throw new KeyNotFoundException($"Equity {createCashDto.EquityId} not found");
        }
        
        // Check if equity already has cash (one-to-one constraint)
        var existingCash = await cashRepository.GetByEquityIdAsync(createCashDto.EquityId);
        if (existingCash.Any())
        {
            logger.LogWarning("Failed to create cash: Equity {EquityId} already has cash", createCashDto.EquityId);
            throw new InvalidOperationException($"Equity {createCashDto.EquityId} already has a Cash");
        }

        var cash = new Cash
        {
            CashAmount = createCashDto.CashAmount,
            EquityId = createCashDto.EquityId
        };
        
        var createdCash = await cashRepository.AddAsync(cash);
        
        logger.LogInformation("Created cash {CashId} for Equity {EquityId}", createdCash.Id, createdCash.EquityId);
        
        return MapToDto(createdCash);
    }

    // ==================== PUT VALIDATION ==================== //
    
    /// <summary>
    /// Updates an existing cash record after validating its existence.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if cash record does not exist.</exception>
    public async Task<CashDto> UpdateCashAsync(long id, UpdateCashDto updateCashDto)
    {
        var cash = await cashRepository.GetByIdAsync(id);
        if (cash == null)
        {
            logger.LogWarning("Failed to update cash {CashId}: not found", id);
            throw new KeyNotFoundException($"Cash with ID {id} not found");
        }
        
        if (updateCashDto.CashAmount.HasValue)
            cash.CashAmount = updateCashDto.CashAmount.Value;

        await cashRepository.UpdateAsync(cash);
        
        logger.LogInformation("Updated cash {CashId}", cash.Id);
        
        return MapToDto(cash);
    }

    // ==================== DELETE VALIDATION ==================== //
    
    /// <summary>
    /// Deletes a cash record after validating its existence.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if cash record does not exist.</exception>
    public async Task DeleteCashAsync(long id)
    {
        var exists = await cashRepository.ExistsAsync(id);
        if (!exists)
        {
            logger.LogWarning("Failed to delete cash {CashId}: not found", id);
            throw new KeyNotFoundException($"Cash {id} not found");
        }
        
        await cashRepository.DeleteAsync(id);
        logger.LogInformation("Deleted cash {CashId}", id);
    }

    // ==================== DTO MAPPING ==================== //
    
    /// <summary>
    /// Maps a <see cref="Cash"/> entity to <see cref="CashDto"/>.
    /// </summary>
    private static CashDto MapToDto(Cash cash)
    {
        return new CashDto
        {
            Id = cash.Id,
            CashAmount = cash.CashAmount,
            EquityId = cash.EquityId,
        };
    }
}