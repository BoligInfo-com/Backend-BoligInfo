using BoligInfo.Core.Models;

namespace BoligInfo.EquityRepository;

public interface IEquityRepository
{
    Task<IEnumerable<Equity>> GetAllAsync();
    Task<Equity?> GetByIdAsync(long id);
    Task<Equity?> GetByIdWithLoansAsync(long id);
    Task<Equity?> GetByIdWithCashAsync(long id);
    Task<Equity> AddAsync(Equity equity);
    Task UpdateAsync(Equity equity);
    Task DeleteAsync(long id);
    Task SaveChangesAsync();
    Task<bool> ExistsAsync(long id);
}