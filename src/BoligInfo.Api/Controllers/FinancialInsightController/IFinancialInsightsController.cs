using BoligInfo.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BoligInfo.Api.Controllers.FinancialInsightController;

public interface IFinancialInsightsController
{
    public Task<ActionResult<HouseFinancialSummaryDto>> GetHouseSummary(long houseId);
    public Task<ActionResult<EquityFinancialSummaryDto>> GetEquitySummary(long equityId);
}