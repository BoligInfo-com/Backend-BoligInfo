using BoligInfo.Core.Models;

namespace BoligInfo.HouseRepository;

public interface IHouseRepository
{
    Task<IEnumerable<House>> GetAllAsync();
    Task<House?> GetByIdAsync(long id);
    Task<IEnumerable<House>> GetByEquityIdAsync(long equityId);
    Task<House?> GetByIdWithCashFlowsAsync(long id);
    Task<House> AddAsync(House house);
    Task UpdateAsync(House house);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}