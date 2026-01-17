using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.HouseRepository;
using BoligInfo.CashFlowRepository;

namespace BoligInfo.HouseService;

public class HouseService(
    IHouseRepository houseRepository,
    IEquityRepository equityRepository,
    ICashFlowRepository cashFlowRepository) : IHouseService
{
    public async Task<IEnumerable<HouseDto>> GetAllHousesAsync()
    {
        var houses = await houseRepository.GetAllAsync();
        return houses.Select(MapToDto);
    }

    public async Task<HouseDto?> GetHouseByIdAsync(long id)
    {
        var house = await houseRepository.GetByIdAsync(id);
        return house == null ? null : MapToDto(house);
    }

    public async Task<IEnumerable<HouseDto>> GetHousesByEquityIdAsync(long equityId)
    {
        var houses = await houseRepository.GetByEquityIdAsync(equityId);
        return houses?.Select(MapToDto) ?? [];
    }

    public async Task<HouseDto?> GetHouseWithCashFlowsAsync(long id)
    {
        var house = await houseRepository.GetByIdWithCashFlowsAsync(id);
        return house == null ? null : MapToDtoWithCashFlows(house);
    }

    public async Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto)
    {
        var equityExists = await equityRepository.ExistsAsync(createHouseDto.EquityId);
        if (!equityExists)
            throw new KeyNotFoundException($"Equity {createHouseDto.EquityId} not found");

        var house = new House
        {
            Price = createHouseDto.Price,
            NumberOfRooms = createHouseDto.NumberOfRooms,
            SquareMeters = createHouseDto.SquareMeters,
            EnergyLabel = string.IsNullOrEmpty(createHouseDto.EnergyLabel)
                ? null
                : Enum.Parse<EnergyLabel>(createHouseDto.EnergyLabel),
            PurchaseDate = createHouseDto.PurchaseDate,
            EquityId = createHouseDto.EquityId
        };

        var createdHouse = await houseRepository.AddAsync(house);
        return MapToDto(createdHouse);
    }

    public async Task<HouseDto> UpdateHouseAsync(long id, UpdateHouseDto updateHouseDto)
    {
        var house = await houseRepository.GetByIdAsync(id);
        if (house == null)
            throw new KeyNotFoundException($"House with ID {id} not found");

        if (updateHouseDto.Price.HasValue)
            house.Price = updateHouseDto.Price.Value;

        if (updateHouseDto.NumberOfRooms.HasValue)
            house.NumberOfRooms = updateHouseDto.NumberOfRooms.Value;

        if (updateHouseDto.SquareMeters.HasValue)
            house.SquareMeters = updateHouseDto.SquareMeters.Value;

        if (updateHouseDto.EnergyLabel != null)
            house.EnergyLabel = Enum.Parse<EnergyLabel>(updateHouseDto.EnergyLabel);

        if (updateHouseDto.PurchaseDate.HasValue)
            house.PurchaseDate = updateHouseDto.PurchaseDate.Value;

        await houseRepository.UpdateAsync(house);
        return MapToDto(house);
    }

    public async Task DeleteHouseAsync(long id)
    {
        var house = await houseRepository.GetByIdWithCashFlowsAsync(id);
        if (house == null)
            throw new KeyNotFoundException($"House with ID {id} not found");

        // Delete all cash flows associated with this house
        if (house.CashFlows != null && house.CashFlows.Any())
        {
            foreach (var cashFlow in house.CashFlows.ToList())
            {
                await cashFlowRepository.DeleteAsync(cashFlow.Id);
            }
        }

        await houseRepository.DeleteAsync(id);
    }

    private static HouseDto MapToDto(House house)
    {
        return new HouseDto
        {
            Id = house.Id,
            Price = house.Price,
            NumberOfRooms = house.NumberOfRooms,
            SquareMeters = house.SquareMeters,
            EnergyLabel = house.EnergyLabel?.ToString(),
            PurchaseDate = house.PurchaseDate,
            EquityId = house.EquityId
        };
    }

    private static HouseDto MapToDtoWithCashFlows(House house)
    {
        return new HouseDto
        {
            Id = house.Id,
            Price = house.Price,
            NumberOfRooms = house.NumberOfRooms,
            SquareMeters = house.SquareMeters,
            EnergyLabel = house.EnergyLabel?.ToString(),
            PurchaseDate = house.PurchaseDate,
            EquityId = house.EquityId,
            CashFlows = house.CashFlows?.Select(cf => new CashFlowDto
            {
                Id = cf.Id,
                Type = cf.Type.ToString(),
                Frequency = cf.Frequency.ToString(),
                Amount = cf.Amount,
                Name = cf.Name,
                Description = cf.Description,
                HouseId = cf.HouseId
            }).ToList()
        };
    }
}