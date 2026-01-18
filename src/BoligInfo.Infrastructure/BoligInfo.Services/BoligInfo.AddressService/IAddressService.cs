using BoligInfo.Core.DTO;

namespace BoligInfo.AddressService;

public interface IAddressService
{
    Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
    Task<AddressDto?> GetAddressByIdAsync(long id);
    Task<AddressDto?> GetAddressByHouseIdAsync(long houseId);
    Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto);
    Task<AddressDto> UpdateAddressAsync(long id, UpdateAddressDto updateAddressDto);
    Task DeleteAddressAsync(long id);
}