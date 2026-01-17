using Boliginfo.CashRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.LoanRepository;
using Microsoft.Extensions.Logging;

namespace BoligInfo.EquityService;

/// <summary>
/// Service for managing <see cref="Equity"/> entities.
/// Handles validation, business logic, and DTO mapping.
/// </summary>
public class EquityService(
    IEquityRepository equityRepository,
    ILoanRepository loanRepository,
    ICashRepository cashRepository,
    ILogger<EquityService> logger
    ) : IEquityService
{
    // ==================== GET VALIDATION ==================== //
    
    /// <summary>
    /// Retrieves all equities and maps them to DTOs.
    /// </summary>
    public async Task<IEnumerable<EquityDto>> GetAllEquitiesAsync()
    {
        var equities = await equityRepository.GetAllAsync();

        var enumerable = equities.ToList();
        logger.LogInformation("Retrieved {Count} equities", enumerable.Count);
        
        return enumerable.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves an equity by ID.
    /// Returns null if not found.
    /// </summary>
    public async Task<EquityDto?> GetEquityByIdAsync(long id)
    {
        var equity = await equityRepository.GetByIdAsync(id);
        
        if (equity == null) logger.LogWarning("Equity {EquityId} not found", id);
        
        return equity == null ? null : MapToDto(equity);
    }

    /// <summary>
    /// Retrieves an equity with all associated loans.
    /// Returns null if not found.
    /// </summary>
    public async Task<EquityDto?> GetEquityWithLoansAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithLoansAsync(id);
        return equity == null ? null : MapToDtoWithLoans(equity);
    }

    /// <summary>
    /// Retrieves an equity with its associated cash.
    /// Returns null if not found.
    /// </summary>
    public async Task<EquityDto?> GetEquityWithCashAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithCashAsync(id);
        return equity == null ? null : MapToDtoWithCash(equity);
    }

    // ==================== POST VALIDATION ==================== //
    
    /// <summary>
    /// Creates a new equity with optional currency (default "DKK").
    /// </summary>
    public async Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto)
    {
        var equity = new Equity
        {
            Currency = createEquityDto.Currency ?? "DKK"
        };

        var createdEquity = await equityRepository.AddAsync(equity);
        
        logger.LogInformation("Created equity {EquityId}", createdEquity.Id);
        
        return MapToDto(createdEquity);
    }

    // ==================== PUT VALIDATION ==================== //
    
    /// <summary>
    /// Updates an existing equity.
    /// Throws <see cref="KeyNotFoundException"/> if equity does not exist.
    /// </summary>
    public async Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto)
    {
        var equity = await equityRepository.GetByIdAsync(id);
        if (equity == null)
        {
            logger.LogWarning("Failed to update equity {EquityId}: not found", id);
            throw new KeyNotFoundException($"Equity with ID {id} not found");
        }
        
        if (updateEquityDto.Currency != null)
            equity.Currency = updateEquityDto.Currency;

        await equityRepository.UpdateAsync(equity);
        
        logger.LogInformation("Updated equity {EquityId}", equity.Id);
        
        return MapToDto(equity);
    }

    // ==================== DELETE VALIDATION ==================== //
    
    /// <summary>
    /// Deletes an equity and its related loans and cash.
    /// Throws <see cref="KeyNotFoundException"/> if equity does not exist.
    /// </summary>
    public async Task DeleteEquityAsync(long id)
    {
        var equity = await equityRepository.GetByIdWithLoansAsync(id);
        if (equity == null)
        {
            logger.LogWarning("Failed to delete equity {EquityId}: not found", id);
            throw new KeyNotFoundException($"Equity with ID {id} not found");
        }
        
        // Delete related loans for in-memory DB (production DB uses cascade delete)
        if (equity.Loans != null && equity.Loans.Count != 0)
        {
            foreach (var loan in equity.Loans.ToList())
            {
                await loanRepository.DeleteAsync(loan.Id);
            }
        }
        
        // Delete cash if exists
        var equityWithCash = await equityRepository.GetByIdWithCashAsync(id);
        if (equityWithCash?.Cash != null)
            await cashRepository.DeleteAsync(equityWithCash.Cash.Id);
        
        
        await equityRepository.DeleteAsync(id);
        logger.LogInformation("Deleted equity {EquityId}", id);
    }

    // ==================== DTO MAPPING ==================== //
    
    /// <summary>
    /// Maps an <see cref="Equity"/> entity to a simplified <see cref="EquityDto"/>.
    /// Only includes the equity's basic properties (ID and Currency).
    /// </summary>
    /// <param name="equity">The equity entity to map.</param>
    /// <returns>A <see cref="EquityDto"/> containing ID and Currency.</returns>
    private static EquityDto MapToDto(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency
        };
    }

    /// <summary>
    /// Maps an <see cref="Equity"/> entity to an <see cref="EquityDto"/> including its related loans.
    /// Each loan is mapped to <see cref="LoanDto"/>.
    /// </summary>
    /// <param name="equity">The equity entity to map.</param>
    /// <returns>An <see cref="EquityDto"/> containing ID, Currency, and a list of associated <see cref="LoanDto"/>.</returns>
    private static EquityDto MapToDtoWithLoans(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency,
            Loans = equity.Loans?.Select(l => new LoanDto
            {
                Id = l.Id,
                LoanType = l.LoanType.ToString(),
                LoanAmount = l.LoanAmount,
                InterestRate = l.InterestRate,
                LoanLifetime = l.LoanLifetime,
                EquityId = l.EquityId
            }).ToList()
        };
    }
    
    /// <summary>
    /// Maps an <see cref="Equity"/> entity to an <see cref="EquityDto"/> including its related cash.
    /// The cash is mapped to the <see cref="Cash"/> property of the DTO.
    /// </summary>
    /// <param name="equity">The equity entity to map.</param>
    /// <returns>An <see cref="EquityDto"/> containing ID, Currency, and the associated <see cref="Cash"/> if present.</returns>
    private static EquityDto MapToDtoWithCash(Equity equity)
    {
        return new EquityDto
        {
            Id = equity.Id,
            Currency = equity.Currency,
            Cash = equity.Cash != null ? new Cash
            {
                Id = equity.Cash.Id,
                CashAmount = equity.Cash.CashAmount,
                EquityId = equity.Cash.EquityId
            } : null
        };
    }
}