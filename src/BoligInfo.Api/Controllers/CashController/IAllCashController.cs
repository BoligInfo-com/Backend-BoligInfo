using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashController;

public interface IAllCashController
{
    Task<ActionResult<IEnumerable<CashDto>>> GetAll();
    Task<ActionResult<CashDto>> GetById(long id);
    Task<ActionResult<IEnumerable<CashDto>>> GetByEquityId(long equityId);
    Task<ActionResult<CashDto>> Create(CreateCashDto createCashDto);
    Task<ActionResult<CashDto>> Update(long id, UpdateCashDto updateCashDto);
    Task<ActionResult> Delete(long id);
}