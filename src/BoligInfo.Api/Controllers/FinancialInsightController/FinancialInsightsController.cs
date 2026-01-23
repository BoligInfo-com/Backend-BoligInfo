using BoligInfo.BusinessLogic.FinancialInsightService;
using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.FinancialInsightController;

[ApiController]
[Route("api/financial-insights")]
public class FinancialInsightsController(
    IFinancialInsightService service
) : ControllerBase
{
    [HttpGet("house/{houseId:long}")]
    public async Task<ActionResult<HouseFinancialSummaryDto>> GetHouseSummary(long houseId)
    {
        try
        {
            return Ok(await service.GetHouseSummaryAsync(houseId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("equity/{equityId:long}")]
    public async Task<ActionResult<EquityFinancialSummaryDto>> GetEquitySummary(long equityId)
    {
        try
        {
            return Ok(await service.GetEquitySummaryAsync(equityId));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
