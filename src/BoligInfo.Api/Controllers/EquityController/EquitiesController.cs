using BoligInfo.Core.DTO;
using BoligInfo.EquityService;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.EquityController;

[ApiController]
[Route("api/[controller]")]
public class EquitiesController(IEquityService equityService) : ControllerBase, IEquitiesController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquityDto>>> GetAll()
    {
        var equities = await equityService.GetAllEquitiesAsync();
        return Ok(equities);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EquityDto>> GetById(long id)
    {
        var equity = await equityService.GetEquityByIdAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    [HttpGet("{id:long}/with-loans")]
    public async Task<ActionResult<EquityDto>> GetByIdWithLoans(long id)
    {
        var equity = await equityService.GetEquityWithLoansAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    [HttpPost]
    public async Task<ActionResult<EquityDto>> Create(CreateEquityDto createEquityDto)
    {
        try
        {
            var equity = await equityService.CreateEquityAsync(createEquityDto);
            return CreatedAtAction(nameof(GetById), new { id = equity.Id }, equity);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<EquityDto>> Update(long id, UpdateEquityDto updateEquityDto)
    {
        try
        {
            var equity = await equityService.UpdateEquityAsync(id, updateEquityDto);
            return Ok(equity);
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
            await equityService.DeleteEquityAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();   
        }
    }
}