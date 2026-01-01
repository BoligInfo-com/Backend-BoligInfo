using BoligInfo.Core.DTO;

namespace BoligInfo.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllLoansAsync();
    Task<LoanDto?> GetLoanByIdAsync(long id);
    Task<IEnumerable<LoanDto>> GetLoansByEquityIdAsync(long equityId);
    Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
    Task<LoanDto> UpdateLoanAsync(long id, UpdateLoanDto updateLoanDto);
    Task DeleteLoanAsync(long id);
}