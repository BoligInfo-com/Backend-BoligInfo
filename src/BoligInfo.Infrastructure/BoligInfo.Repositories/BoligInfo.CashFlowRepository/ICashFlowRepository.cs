using BoligInfo.Core.Models;

namespace BoligInfo.CashFlowRepository;

public interface ICashFlowRepository
{
    Task<IEnumerable<CashFlow>> GetAllAsync();
    Task<CashFlow?> GetByIdAsync(long id);
    Task<IEnumerable<CashFlow>> GetByHouseIdAsync(long houseId);
    Task<CashFlow> AddAsync(CashFlow cashFlow);
    Task UpdateAsync(CashFlow cashFlow);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}