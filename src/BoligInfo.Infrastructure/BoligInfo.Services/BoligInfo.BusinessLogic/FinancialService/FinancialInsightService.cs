using BoligInfo.BusinessLogic.CashFlowConversionHelper;
using BoligInfo.Core.DTO;
using BoligInfo.Core.Enums;
using BoligInfo.HouseRepository;
using Microsoft.Extensions.Logging;

namespace BoligInfo.BusinessLogic.FinancialService;

public class FinancialInsightService(
    IHouseRepository houseRepository,
    ICashFlowConversionHelper  cashFlowConversionHelper,
    ILogger<FinancialInsightService> logger
) : IFinancialInsightService
{
    public async Task<HouseFinancialSummaryDto> GetHouseSummaryAsync(long houseId)
    {
        logger.LogInformation("Calculating financial summary for House {HouseId}", houseId);

        var house = await houseRepository.GetByIdWithCashFlowsAsync(houseId);
        if (house == null)
        {
            logger.LogWarning("House {HouseId} not found", houseId);
            throw new KeyNotFoundException();
        }

        var cashFlows = house.CashFlows ?? [];

        var income = cashFlows
            .Where(cf => cf.Type == CashFlowType.INCOME)
            .Sum(cashFlowConversionHelper.ToMonthlyAmount);

        var expenses = cashFlows
            .Where(cf => cf.Type == CashFlowType.EXPENSE)
            .Sum(cashFlowConversionHelper.ToMonthlyAmount);

        return new HouseFinancialSummaryDto
        {
            HouseId = houseId,
            MonthlyIncome = income,
            MonthlyExpenses = expenses,
            NetMonthlyCashFlow = income - expenses
        };
    }

    public async Task<EquityFinancialSummaryDto> GetEquitySummaryAsync(long equityId)
    {
        logger.LogInformation("Calculating financial summary for Equity {EquityId}", equityId);

        var houses = await houseRepository.GetByEquityIdAsync(equityId);

        var summaries = new List<HouseFinancialSummaryDto>();

        foreach (var house in houses)
        {
            var summary = await GetHouseSummaryAsync(house.Id);
            summaries.Add(summary);
        }

        return new EquityFinancialSummaryDto
        {
            EquityId = equityId,
            NumberOfHouses = summaries.Count,
            TotalMonthlyIncome = summaries.Sum(s => s.MonthlyIncome),
            TotalMonthlyExpenses = summaries.Sum(s => s.MonthlyExpenses),
            NetMonthlyCashFlow = summaries.Sum(s => s.NetMonthlyCashFlow)
        };
    }
}