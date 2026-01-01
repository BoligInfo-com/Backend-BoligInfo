using BoligInfo.Core.DTO;

namespace BoligInfo.Services;

public interface IEquityService
{
    Task<IEnumerable<EquityDto>> GetAllEquitiesAsync();
    Task<EquityDto?> GetEquityByIdAsync(long id);
    Task<EquityDto?> GetEquityWithLoansAsync(long id);
    Task<EquityDto> CreateEquityAsync(CreateEquityDto createEquityDto);
    Task<EquityDto> UpdateEquityAsync(long id, UpdateEquityDto updateEquityDto);
    Task DeleteEquityAsync(long id);
}