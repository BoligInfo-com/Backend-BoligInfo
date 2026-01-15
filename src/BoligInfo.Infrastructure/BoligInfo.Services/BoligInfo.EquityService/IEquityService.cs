using BoligInfo.Core.DTO;

namespace BoligInfo.EquityService;

public interface IEquityService
{
    Task<IEnumerable<EquityDto>> GetAllEquitiesAsync();
    Task<EquityDto?> GetEquityByIdAsync(long id);
    Task<EquityDto?> GetEquityWithLoansAsync(long id);
    Task<EquityDto?> GetEquityWithCashAsync(long id);
    Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto);
    Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto);
    Task DeleteEquityAsync(long id);
    
}