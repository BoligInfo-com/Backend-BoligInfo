using BoligInfo.Core.DTO;

namespace BoligInfo.BusinessLogic.FinancialInsightService;

public interface IFinancialInsightService
{
    Task<HouseFinancialSummaryDto> GetHouseSummaryAsync(long houseId);
    Task<EquityFinancialSummaryDto> GetEquitySummaryAsync(long equityId);
}