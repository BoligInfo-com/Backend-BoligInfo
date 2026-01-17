using Boliginfo.CashRepository;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Models;
using BoligInfo.EquityRepository;

namespace BoligInfo.CashService;

public class CashService(ICashRepository cashRepository, IEquityRepository equityRepository) : ICashService
{
    public async Task<IEnumerable<CashDto>> GetAllCashAsync()
    {
        var allCash = await cashRepository.GetAllAsync();
        return allCash.Select(MapToDto);
    }

    public async Task<CashDto?> GetCashByIdAsync(long id)
    {
        var cash = await cashRepository.GetByIdAsync(id);
        return cash == null ? null : MapToDto(cash);
    }

    public async Task<IEnumerable<CashDto>> GetAllCashByEquityIdAsync(long equityId)
    {
        var allCash = await cashRepository.GetByEquityIdAsync(equityId);
        return allCash.Select(MapToDto); 
    }

    public async Task<CashDto> CreateCashAsync(CreateCashDto createCashDto)
    {
        // Ensure parent Equity exists
        var equityExists  = await equityRepository.ExistsAsync(createCashDto.EquityId);
        if (!equityExists)
            throw new KeyNotFoundException($"Equity {createCashDto.EquityId} not found");
        
        // Check if equity already has cash (one-to-one constraint)
        var existingCash = await cashRepository.GetByEquityIdAsync(createCashDto.EquityId);
        if (existingCash.Any())
            throw new InvalidOperationException($"Equity {createCashDto.EquityId} already has a Cash");
        

        var cash = new Cash
        {
            CashAmount = createCashDto.CashAmount,
            EquityId = createCashDto.EquityId
        };
        
        var createdCash = await cashRepository.AddAsync(cash);
        return MapToDto(createdCash);
    }

    public async Task<CashDto> UpdateCashAsync(long id, UpdateCashDto updateCashDto)
    {
        var cash = await cashRepository.GetByIdAsync(id);
        if (cash == null)
            throw new KeyNotFoundException($"Cash with ID {id} not found");
        
        if (updateCashDto.CashAmount.HasValue)
            cash.CashAmount = updateCashDto.CashAmount.Value;

        await cashRepository.UpdateAsync(cash);
        return MapToDto(cash);
    }

    public async Task DeleteCashAsync(long id)
    {
        var exists = await cashRepository.ExistsAsync(id);
        if (!exists)
            throw new KeyNotFoundException($"Cash {id} not found");
        
        await cashRepository.DeleteAsync(id);
    }

    private static CashDto MapToDto(Cash cash)
    {
        return new CashDto
        {
            Id = cash.Id,
            CashAmount = cash.CashAmount,
            EquityId = cash.EquityId,
        };
    }
}