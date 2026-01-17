using BoligInfo.Core.DTO;

namespace BoligInfo.CashFlowService;

public interface ICashFlowService
{
    Task<IEnumerable<CashFlowDto>> GetAllCashFlowsAsync();
    Task<CashFlowDto?> GetCashFlowByIdAsync(long id);
    Task<IEnumerable<CashFlowDto>> GetCashFlowsByHouseIdAsync(long houseId);
    Task<CashFlowDto> CreateCashFlowAsync(CreateCashFlowDto createCashFlowDto);
    Task<CashFlowDto> UpdateCashFlowAsync(long id, UpdateCashFlowDto updateCashFlowDto);
    Task DeleteCashFlowAsync(long id);
}