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
        var equity = await equityRepository.GetByIdAsync(createCashDto.EquityId);
        if (equity == null)
            throw new KeyNotFoundException($"Equity {createCashDto.EquityId} not found");
        
        // Enforce 1:1 Cash
        equity.Cash = new Cash
        {
            CashAmount = createCashDto.CashAmount,
        };

        await cashRepository.SaveChangesAsync();
        
        return MapToDto(equity.Cash);
    }

    public async Task<CashDto> UpdateCashAsync(long id, UpdateCashDto updateCashDto)
    {
        var cash = await cashRepository.GetByIdAsync(id);
        if (cash == null)
            throw new KeyNotFoundException($"Cash with ID {id} not found");
        
        if (updateCashDto.CashAmount.HasValue)
            cash.CashAmount = updateCashDto.CashAmount.Value;

        await cashRepository.SaveChangesAsync();
        return MapToDto(cash);
    }

    public async Task DeleteCashAsync(long id)
    {
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