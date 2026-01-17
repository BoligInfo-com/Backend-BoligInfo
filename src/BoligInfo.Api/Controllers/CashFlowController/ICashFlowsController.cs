using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashFlowController;

public interface ICashFlowsController
{
    Task<ActionResult<IEnumerable<CashFlowDto>>> GetAll();
    Task<ActionResult<CashFlowDto>> GetById(long id);
    Task<ActionResult<IEnumerable<CashFlowDto>>> GetByHouseId(long houseId);
    Task<ActionResult<CashFlowDto>> Create(CreateCashFlowDto createCashFlowDto);
    Task<ActionResult<CashFlowDto>> Update(long id, UpdateCashFlowDto updateCashFlowDto);
    Task<ActionResult> Delete(long id);
}