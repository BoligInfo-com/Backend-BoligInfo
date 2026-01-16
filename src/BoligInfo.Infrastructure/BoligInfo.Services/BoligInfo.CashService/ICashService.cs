using BoligInfo.Core.DTO;

namespace BoligInfo.CashService;

public interface ICashService
{
    Task<IEnumerable<CashDto>> GetAllCashAsync();
    Task<CashDto?> GetCashByIdAsync(long id);
    Task<IEnumerable<CashDto>> GetAllCashByEquityIdAsync(long equityId);
    Task<CashDto> CreateCashAsync(CreateCashDto createCashDto);
    Task<CashDto> UpdateCashAsync(long id, UpdateCashDto updateCashDto);
    Task DeleteCashAsync(long id);
}