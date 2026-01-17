using BoligInfo.CashFlowService;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashFlowController;

[ApiController]
[Route("api/[controller]")]
public class CashFlowsController(ICashFlowService cashFlowService) : ControllerBase, ICashFlowsController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CashFlowDto>>> GetAll()
    {
        var cashFlows = await cashFlowService.GetAllCashFlowsAsync();
        return Ok(cashFlows);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CashFlowDto>> GetById(long id)
    {
        var cashFlow = await cashFlowService.GetCashFlowByIdAsync(id);
        if (cashFlow == null)
            return NotFound();
        
        return Ok(cashFlow);
    }

    [HttpGet("house/{houseId:long}")]
    public async Task<ActionResult<IEnumerable<CashFlowDto>>> GetByHouseId(long houseId)
    {
        var cashFlows = await cashFlowService.GetCashFlowsByHouseIdAsync(houseId);
        return Ok(cashFlows);
    }

    [HttpPost]
    public async Task<ActionResult<CashFlowDto>> Create(CreateCashFlowDto createCashFlowDto)
    {
        try
        {
            var cashFlow = await cashFlowService.CreateCashFlowAsync(createCashFlowDto);
            return CreatedAtAction(nameof(GetById), new { id = cashFlow.Id }, cashFlow);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CashFlowDto>> Update(long id, UpdateCashFlowDto updateCashFlowDto)
    {
        try
        {
            var cashFlow = await cashFlowService.UpdateCashFlowAsync(id, updateCashFlowDto);
            return Ok(cashFlow);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult> Delete(long id)
    {
        try
        {
            await cashFlowService.DeleteCashFlowAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }