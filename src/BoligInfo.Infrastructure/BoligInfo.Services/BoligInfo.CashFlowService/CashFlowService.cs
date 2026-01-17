using BoligInfo.CashFlowRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using BoligInfo.HouseRepository;

namespace BoligInfo.CashFlowService;

public class CashFlowService(
    ICashFlowRepository cashFlowRepository,
    IHouseRepository houseRepository) : ICashFlowService
{
    public async Task<IEnumerable<CashFlowDto>> GetAllCashFlowsAsync()
    {
        var cashFlows = await cashFlowRepository.GetAllAsync();
        return cashFlows.Select(MapToDto);
    }

    public async Task<CashFlowDto?> GetCashFlowByIdAsync(long id)
    {
        var cashFlow = await cashFlowRepository.GetByIdAsync(id);
        return cashFlow == null ? null : MapToDto(cashFlow);
    }

    public async Task<IEnumerable<CashFlowDto>> GetCashFlowsByHouseIdAsync(long houseId)
    {
        var cashFlows = await cashFlowRepository.GetByHouseIdAsync(houseId);
        return cashFlows?.Select(MapToDto) ?? [];
    }

    public async Task<CashFlowDto> CreateCashFlowAsync(CreateCashFlowDto createCashFlowDto)
    {
        var houseExists = await houseRepository.ExistsAsync(createCashFlowDto.HouseId);
        if (!houseExists)
            throw new KeyNotFoundException($"House {createCashFlowDto.HouseId} not found");

        var cashFlow = new CashFlow
        {
            Type = Enum.Parse<CashFlowType>(createCashFlowDto.Type),
            Frequency = Enum.Parse<Frequency>(createCashFlowDto.Frequency),
            Amount = createCashFlowDto.Amount,
            Name = createCashFlowDto.Name,
            Description = createCashFlowDto.Description,
            HouseId = createCashFlowDto.HouseId
        };

        var createdCashFlow = await cashFlowRepository.AddAsync(cashFlow);
        return MapToDto(createdCashFlow);
    }

    public async Task<CashFlowDto> UpdateCashFlowAsync(long id, UpdateCashFlowDto updateCashFlowDto)
    {
        var cashFlow = await cashFlowRepository.GetByIdAsync(id);
        if (cashFlow == null)
            throw new KeyNotFoundException($"CashFlow with ID {id} not found");

        if (updateCashFlowDto.Type != null)
            cashFlow.Type = Enum.Parse<CashFlowType>(updateCashFlowDto.Type);

        if (updateCashFlowDto.Frequency != null)
            cashFlow.Frequency = Enum.Parse<Frequency>(updateCashFlowDto.Frequency);

        if (updateCashFlowDto.Amount.HasValue)
            cashFlow.Amount = updateCashFlowDto.Amount.Value;

        if (updateCashFlowDto.Name != null)
            cashFlow.Name = updateCashFlowDto.Name;

        if (updateCashFlowDto.Description != null)
            cashFlow.Description = updateCashFlowDto.Description;

        await cashFlowRepository.UpdateAsync(cashFlow);
        return MapToDto(cashFlow);
    }

    public async Task DeleteCashFlowAsync(long id)
    {
        var exists = await cashFlowRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"CashFlow {id} not found");

        await cashFlowRepository.DeleteAsync(id);
    }

    private static CashFlowDto MapToDto(CashFlow cashFlow)
    {
        return new CashFlowDto
        {
            Id = cashFlow.Id,
            Type = cashFlow.Type.ToString(),
            Frequency = cashFlow.Frequency.ToString(),
            Amount = cashFlow.Amount,
            Name = cashFlow.Name,
            Description = cashFlow.Description,
            HouseId = cashFlow.HouseId
        };
    }
}