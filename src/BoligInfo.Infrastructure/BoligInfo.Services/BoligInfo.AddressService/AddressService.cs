using BoligInfo.AddressRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.HouseRepository;

namespace BoligInfo.AddressService;

public class AddressService(
    IAddressRepository addressRepository,
    IHouseRepository houseRepository) : IAddressService
{
    public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
    {
        var addresses = await addressRepository.GetAllAsync();
        return addresses.Select(MapToDto);
    }

    public async Task<AddressDto?> GetAddressByIdAsync(long id)
    {
        var address = await addressRepository.GetByIdAsync(id);
        return address == null ? null : MapToDto(address);
    }

    public async Task<AddressDto?> GetAddressByHouseIdAsync(long houseId)
    {
        var address = await addressRepository.GetByHouseIdAsync(houseId);
        return address == null ? null : MapToDto(address);
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto)
    {
        // Ensure parent House exists
        var houseExists = await houseRepository.ExistsAsync(createAddressDto.HouseId);
        if (!houseExists)
            throw new KeyNotFoundException($"House {createAddressDto.HouseId} not found");

        // Check if house already has an address (one-to-one constraint)
        var existingAddress = await addressRepository.GetByHouseIdAsync(createAddressDto.HouseId);
        if (existingAddress != null)
            throw new InvalidOperationException($"House {createAddressDto.HouseId} already has an Address");

        var address = new Address
        {
            Country = createAddressDto.Country ?? "Denmark",
            City = createAddressDto.City,
            Zipcode = createAddressDto.Zipcode,
            Street = createAddressDto.Street,
            Number = createAddressDto.Number,
            Suite = createAddressDto.Suite,
            Floor = createAddressDto.Floor,
            HouseId = createAddressDto.HouseId
        };

        var createdAddress = await addressRepository.AddAsync(address);
        return MapToDto(createdAddress);
    }

    public async Task<AddressDto> UpdateAddressAsync(long id, UpdateAddressDto updateAddressDto)
    {
        var address = await addressRepository.GetByIdAsync(id);
        if (address == null)
            throw new KeyNotFoundException($"Address with ID {id} not found");

        if (updateAddressDto.Country != null)
            address.Country = updateAddressDto.Country;

        if (updateAddressDto.City != null)
            address.City = updateAddressDto.City;

        if (updateAddressDto.Zipcode != null)
            address.Zipcode = updateAddressDto.Zipcode;

        if (updateAddressDto.Street != null)
            address.Street = updateAddressDto.Street;

        if (updateAddressDto.Number != null)
            address.Number = updateAddressDto.Number;

        if (updateAddressDto.Suite != null)
            address.Suite = updateAddressDto.Suite;

        if (updateAddressDto.Floor.HasValue)
            address.Floor = updateAddressDto.Floor.Value;

        await addressRepository.UpdateAsync(address);
        return MapToDto(address);
    }

    public async Task DeleteAddressAsync(long id)
    {
        var exists = await addressRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Address {id} not found");

        await addressRepository.DeleteAsync(id);
    }

    private static AddressDto MapToDto(Address address)
    {
        return new AddressDto
        {
            Id = address.Id,
            Country = address.Country,
            City = address.City,
            Zipcode = address.Zipcode,
            Street = address.Street,
            Number = address.Number,
            Suite = address.Suite,
            Floor = address.Floor,
            HouseId = address.HouseId
        };
    }
}