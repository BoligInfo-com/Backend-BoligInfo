using BoligInfo.AddressRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;
using BoligInfo.HouseRepository;
using BoligInfo.CashFlowRepository;
using Microsoft.Extensions.Logging;

namespace BoligInfo.HouseService;

/// <summary>
/// Provides business logic for managing houses.
/// </summary>
public class HouseService(
    IHouseRepository houseRepository,
    IEquityRepository equityRepository,
    ICashFlowRepository cashFlowRepository,
    IAddressRepository addressRepository,
    ILogger<HouseService> logger) : IHouseService
{
    /// <inheritdoc />
    public async Task<IEnumerable<HouseDto>> GetAllHousesAsync()
    {
        logger.LogInformation("Fetching all houses");
        
        var houses = await houseRepository.GetAllAsync();
        return houses.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<HouseDto?> GetHouseByIdAsync(long id)
    {
        logger.LogInformation("Fetching house with ID {HouseId}", id);
        
        var house = await houseRepository.GetByIdAsync(id);
        
        if (house == null)
            logger.LogWarning("House with ID {HouseId} not found", id);
        
        return house == null ? null : MapToDto(house);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HouseDto>> GetHousesByEquityIdAsync(long equityId)
    {
        logger.LogInformation("Fetching houses for Equity ID {EquityId}", equityId);
        
        var houses = await houseRepository.GetByEquityIdAsync(equityId);
        return houses.Select(MapToDto);
    }

    /// <inheritdoc />
    public async Task<HouseDto?> GetHouseWithCashFlowsAsync(long id)
    {
        logger.LogInformation("Fetching house with cash flows for House ID {HouseId}", id);
        
        var house = await houseRepository.GetByIdWithCashFlowsAsync(id);
        
        if (house == null)
            logger.LogWarning("House with ID {HouseId} not found", id);
        
        return house == null ? null : MapToDtoWithCashFlows(house);
    }

    /// <inheritdoc />
    public async Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto)
    {
        logger.LogInformation("Creating house for Equity ID {EquityId}", createHouseDto.EquityId);
        
        var equityExists = await equityRepository.ExistsAsync(createHouseDto.EquityId);
        if (!equityExists)
        {
            logger.LogError("Equity with ID {EquityId} not found", createHouseDto.EquityId);
            throw new KeyNotFoundException($"Equity {createHouseDto.EquityId} not found");
        }
        
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
        
        logger.LogInformation("House created with ID {HouseId}", createdHouse.Id);
        
        return MapToDto(createdHouse);
    }

    /// <inheritdoc />
    public async Task<HouseDto> UpdateHouseAsync(long id, UpdateHouseDto updateHouseDto)
    {
        logger.LogInformation("Updating house with ID {HouseId}", id);
        
        var house = await houseRepository.GetByIdAsync(id);
        if (house == null)
        {
            logger.LogWarning("House with ID {HouseId} not found", id);
            throw new KeyNotFoundException($"House with ID {id} not found");
        }
        
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
        
        logger.LogInformation("House with ID {HouseId} updated", id);
        
        return MapToDto(house);
    }

    /// <inheritdoc />
    public async Task DeleteHouseAsync(long id)
    {
        logger.LogInformation("Deleting house with ID {HouseId}", id);
        
        var house = await houseRepository.GetByIdWithCashFlowsAsync(id);
        if (house == null)
        {
            logger.LogWarning("House with ID {HouseId} not found", id);
            throw new KeyNotFoundException($"House with ID {id} not found");
        }
        
        // Delete all cash flows associated with this house
        if (house.CashFlows != null && house.CashFlows.Count != 0)
        {
            logger.LogInformation("Deleting {Count} cash flows for House ID {HouseId}", house.CashFlows.Count, id);
            
            foreach (var cashFlow in house.CashFlows.ToList())
            {
                await cashFlowRepository.DeleteAsync(cashFlow.Id);
            }
        }
        
        // Delete address associated with this house
        var address = await addressRepository.GetByHouseIdAsync(id);
        if (address != null)
        {
            logger.LogInformation("Deleting address with ID {AddressId} for House ID {HouseId}", address.Id, id);
            await addressRepository.DeleteAsync(address.Id);
        }

        await houseRepository.DeleteAsync(id);
        
        logger.LogInformation("House with ID {HouseId} deleted", id);
    }
    
    #region Mapping

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
    
    #endregion
}