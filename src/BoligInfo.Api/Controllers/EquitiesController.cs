using BoligInfo.Core.DTOs;
using BoligInfo.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquitiesController : ControllerBase
{
    private readonly IEquityService _equityService;

    public EquitiesController(IEquityService equityService)
    {
        _equityService = equityService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EquityDto>>> GetAll()
    {
        var equities = await _equityService.GetAllEquitiesAsync();
        return Ok(equities);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EquityDto>> GetById(long id)
    {
        var equity = await _equityService.GetEquityByIdAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    [HttpGet("{id:long}/with-loans")]
    public async Task<ActionResult<EquityDto>> GetByIdWithLoans(long id)
    {
        var equity = await _equityService.GetEquityWithLoansAsync(id);
        if (equity == null)
            return NotFound();
        
        return Ok(equity);
    }

    [HttpPost]
    public async Task<ActionResult<EquityDto>> Create(CreateEquityDto createEquityDto)
    {
        var equity = await _equityService.CreateEquityAsync(createEquityDto);
        return CreatedAtAction(nameof(GetById), new { id = equity.Id }, equity);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<EquityDto>> Update(long id, UpdateEquityDto updateEquityDto)
    {
        try
        {
            var equity = await _equityService.UpdateEquityAsync(id, updateEquityDto);
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
        await _equityService.DeleteEquityAsync(id);
        return NoContent();
    }
}