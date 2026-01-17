using BoligInfo.Core.DTO;

namespace BoligInfo.HouseService;

public interface IHouseService
{
    Task<IEnumerable<HouseDto>> GetAllHousesAsync();
    Task<HouseDto?> GetHouseByIdAsync(long id);
    Task<IEnumerable<HouseDto>> GetHousesByEquityIdAsync(long equityId);
    Task<HouseDto?> GetHouseWithCashFlowsAsync(long id);
    Task<HouseDto> CreateHouseAsync(CreateHouseDto createHouseDto);
    Task<HouseDto> UpdateHouseAsync(long id, UpdateHouseDto updateHouseDto);
    Task DeleteHouseAsync(long id);
}