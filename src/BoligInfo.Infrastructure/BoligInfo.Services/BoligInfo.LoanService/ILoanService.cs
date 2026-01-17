using BoligInfo.Core.DTO;

namespace BoligInfo.LoanService;

/// <summary>
/// Service interface for managing <see cref="Loan"/> entities.
/// Handles business logic, validation, and DTO mapping.
/// </summary>
public interface ILoanService
{
    /// <summary>
    /// Retrieves all loans.
    /// </summary>
    Task<IEnumerable<LoanDto>> GetAllLoansAsync();
    
    /// <summary>
    /// Retrieves a loan by its unique identifier.
    /// Returns null if not found.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    Task<LoanDto?> GetLoanByIdAsync(long id);
    
    /// <summary>
    /// Retrieves all loans associated with a specific equity.
    /// </summary>
    /// <param name="equityId">Equity ID.</param>
    Task<IEnumerable<LoanDto>> GetLoansByEquityIdAsync(long equityId);
    
    /// <summary>
    /// Creates a new loan.
    /// Throws <see cref="KeyNotFoundException"/> if the parent equity does not exist.
    /// </summary>
    /// <param name="createLoanDto">Data for creating the loan.</param>
    Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
    
    /// <summary>
    /// Updates an existing loan.
    /// Throws <see cref="KeyNotFoundException"/> if the loan does not exist.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    /// <param name="updateLoanDto">Data for updating the loan.</param>
    Task<LoanDto> UpdateLoanAsync(long id, UpdateLoanDto updateLoanDto);
    
    /// <summary>
    /// Deletes a loan by its ID.
    /// Throws <see cref="KeyNotFoundException"/> if the loan does not exist.
    /// </summary>
    /// <param name="id">Loan ID.</param>
    Task DeleteLoanAsync(long id);
}