using BoligInfo.Core.Models;

namespace Boliginfo.CashRepository;

public interface ICashRepository
{
    Task<IEnumerable<Cash>> GetAllAsync();
    Task<Cash?> GetByIdAsync(long id);
    Task<IEnumerable<Cash>> GetByEquityIdAsync(long equityId);
    Task<Cash> AddAsync(Cash cash);
    Task UpdateAsync(Cash cash);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}