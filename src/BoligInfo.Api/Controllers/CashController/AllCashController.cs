using BoligInfo.CashService;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.CashController;

[ApiController]
[Route("api/[controller]")]
public class AllCashController(ICashService cashService) : ControllerBase, IAllCashController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CashDto>>> GetAll()
    {
        var allCash = await cashService.GetAllCashAsync();
        return Ok(allCash);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CashDto>> GetById(long id)
    {
        var cash = await cashService.GetCashByIdAsync(id);
        if (cash == null)
            return NotFound();
        
        return Ok(cash);
    }

    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<IEnumerable<CashDto>>> GetByEquityId(long equityId)
    {
        var allCash = await cashService.GetAllCashByEquityIdAsync(equityId);
        return Ok(allCash);
    }

    [HttpPost]
    public async Task<ActionResult<CashDto>> Create(CreateCashDto createCashDto)
    {
        try
        {
            var cash = await cashService.CreateCashAsync(createCashDto);
            return CreatedAtAction(nameof(GetById), new { id = cash.Id }, cash);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CashDto>> Update(long id, UpdateCashDto updateCashDto)
    {
        try
        {
            var cash = await cashService.UpdateCashAsync(id, updateCashDto);
            return Ok(cash);
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
            await cashService.DeleteCashAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}