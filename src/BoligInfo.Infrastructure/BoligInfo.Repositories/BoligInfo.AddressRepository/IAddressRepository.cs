using BoligInfo.Core.Models;

namespace BoligInfo.AddressRepository;

public interface IAddressRepository
{
    Task<IEnumerable<Address>> GetAllAsync();
    Task<Address?> GetByIdAsync(long id);
    Task<Address?> GetByHouseIdAsync(long houseId);
    Task<Address> AddAsync(Address address);
    Task UpdateAsync(Address address);
    Task DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}