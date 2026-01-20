using BoligInfo.Core.DTO;

namespace BoligInfo.BusinessLogic.FinancialService;

public interface IFinancialInsightService
{
    Task<HouseFinancialSummaryDto> GetHouseSummaryAsync(long houseId);
    Task<EquityFinancialSummaryDto> GetEquitySummaryAsync(long equityId);
}