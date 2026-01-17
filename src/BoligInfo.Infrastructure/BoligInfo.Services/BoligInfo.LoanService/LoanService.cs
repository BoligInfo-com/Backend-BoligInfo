using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.LoanRepository;
using Microsoft.Extensions.Logging;

namespace BoligInfo.LoanService;

/// <summary>
/// Service for managing <see cref="Loan"/> entities.
/// Handles validation, business logic, and DTO mapping.
/// </summary>
public class LoanService(
    ILoanRepository loanRepository, 
    IEquityRepository equityRepository, 
    ILogger<LoanService> logger
    ) : ILoanService
{
    // ==================== GET VALIDATION ==================== //
    
    /// <summary>
    /// Retrieves all loans and maps them to DTOs.
    /// </summary>
    public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
    {
        var loans = await loanRepository.GetAllAsync();

        var enumerable = loans.ToList();
        logger.LogInformation("Retrieved {Count} loans", enumerable.Count);
        
        return enumerable.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a loan by ID and maps it to a DTO.
    /// Returns null if not found.
    /// </summary>
    public async Task<LoanDto?> GetLoanByIdAsync(long id)
    {
        var loan = await loanRepository.GetByIdAsync(id);
        
        if (loan == null) logger.LogWarning("Loan {LoanId} not found", id);
        
        return loan == null ? null : MapToDto(loan);
    }

    /// <summary>
    /// Retrieves all loans associated with a specific equity and maps them to DTOs.
    /// </summary>
    public async Task<IEnumerable<LoanDto>> GetLoansByEquityIdAsync(long equityId)
    {
        var loans = await loanRepository.GetByEquityIdAsync(equityId);
        return loans.Select(MapToDto);
    }

    // ==================== POST VALIDATION ==================== //
    
    /// <summary>
    /// Creates a new loan after validating that the parent equity exists.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if equity does not exist.</exception>
    public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
    {
        // Ensure parent Equity exists
        var equityExists = await equityRepository.ExistsAsync(createLoanDto.EquityId);
        if (!equityExists)
        {
            logger.LogWarning("Failed to create loan: Equity {EquityId} not found", createLoanDto.EquityId);
            throw new KeyNotFoundException($"Equity {createLoanDto.EquityId} not found");
        }
        
        var loan = new Loan
        {
            LoanType = string.IsNullOrEmpty(createLoanDto.LoanType) 
                ? null 
                : Enum.Parse<LoanType>(createLoanDto.LoanType),
            LoanAmount = createLoanDto.LoanAmount,
            InterestRate = createLoanDto.InterestRate,
            LoanLifetime = createLoanDto.LoanLifetime,
            EquityId = createLoanDto.EquityId,
        };
        
        var createdLoan = await loanRepository.AddAsync(loan);
        
        logger.LogInformation("Created new loan {LoanId} for Equity {EquityId}", createdLoan.Id, createdLoan.EquityId);
        
        return MapToDto(createdLoan);
    }

    // ==================== PUT VALIDATION ==================== //
    
    /// <summary>
    /// Updates an existing loan after checking if it exists.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if loan does not exist.</exception>

    public async Task<LoanDto> UpdateLoanAsync(long id, UpdateLoanDto updateLoanDto)
    {
        var loan = await loanRepository.GetByIdAsync(id);
        if (loan == null)
        {
            logger.LogWarning("Failed to update loan {LoanId}: not found", id);
            throw new KeyNotFoundException($"Loan with ID {id} not found");
        }

        if (updateLoanDto.LoanType != null)
            loan.LoanType = Enum.Parse<LoanType>(updateLoanDto.LoanType);
        
        if (updateLoanDto.LoanAmount.HasValue)
            loan.LoanAmount = updateLoanDto.LoanAmount.Value;
        
        if (updateLoanDto.InterestRate.HasValue)
            loan.InterestRate = updateLoanDto.InterestRate.Value;
        
        if (updateLoanDto.LoanLifetime.HasValue)
            loan.LoanLifetime = updateLoanDto.LoanLifetime.Value;

        await loanRepository.UpdateAsync(loan);
        
        logger.LogInformation("Updated loan {LoanId}", loan.Id);
        
        return MapToDto(loan);
    }

    // ==================== DELETE VALIDATION ==================== //
    
    /// <summary>
    /// Deletes a loan after verifying it exists.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Thrown if loan does not exist.</exception>
    public async Task DeleteLoanAsync(long id)
    {
        var exists = await loanRepository.ExistsAsync(id);
        if (!exists)
        {
            logger.LogWarning("Failed to delete loan {LoanId}: not found", id);
            throw new KeyNotFoundException($"Loan {id} not found");
        }

        await loanRepository.DeleteAsync(id);
        
        logger.LogInformation("Deleted loan {LoanId}", id);
    }

    // ==================== DTO MAPPING ==================== //
    
    /// <summary>
    /// Maps a <see cref="Loan"/> entity to <see cref="LoanDto"/>.
    /// </summary>
    private static LoanDto MapToDto(Loan loan)
    {
        return new LoanDto
        {
            Id = loan.Id,
            LoanType = loan.LoanType?.ToString() ?? string.Empty,
            LoanAmount = loan.LoanAmount,
            InterestRate = loan.InterestRate,
            LoanLifetime = loan.LoanLifetime,
            EquityId = loan.EquityId
        };
    }
}