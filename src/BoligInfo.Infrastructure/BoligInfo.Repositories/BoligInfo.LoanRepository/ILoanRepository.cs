using BoligInfo.Core.Models;

namespace BoligInfo.LoanRepository;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(long id);
    Task<IEnumerable<Loan>> GetByEquityIdAsync(long equityId);
    Task<Loan> AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}