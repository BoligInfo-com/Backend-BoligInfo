using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers;

public interface IEquitiesController
{
    Task<ActionResult<IEnumerable<EquityDto>>> GetAll();
    Task<ActionResult<EquityDto>> GetById(long id);
    Task<ActionResult<EquityDto>> GetByIdWithLoans(long id);
    Task<ActionResult<EquityDto>> Create(CreateEquityDto createEquityDto);
    Task<ActionResult<EquityDto>> Update(long id, UpdateEquityDto updateEquityDto);
    Task<ActionResult> Delete(long id);
}