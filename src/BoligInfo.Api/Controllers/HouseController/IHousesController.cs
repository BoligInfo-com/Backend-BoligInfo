using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;
namespace BoligInfo.Api.Controllers.HouseController;

public interface IHousesController
{
    Task<ActionResult<IEnumerable<HouseDto>>> GetAll();
    Task<ActionResult<HouseDto>> GetById(long id);
    Task<ActionResult<IEnumerable<HouseDto>>> GetByEquityId(long equityId);
    Task<ActionResult<HouseDto>> GetByIdWithCashFlows(long id);
    Task<ActionResult<HouseDto>> Create(CreateHouseDto createHouseDto);
    Task<ActionResult<HouseDto>> Update(long id, UpdateHouseDto updateHouseDto);
    Task<ActionResult> Delete(long id);
}
