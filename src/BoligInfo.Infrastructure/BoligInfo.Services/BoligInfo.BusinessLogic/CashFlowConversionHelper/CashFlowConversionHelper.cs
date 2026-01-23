using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;
using Microsoft.Extensions.Logging;

namespace BoligInfo.BusinessLogic.CashFlowConversionHelper;

public class CashFlowConversionHelper(
    ILogger<CashFlowConversionHelper> logger
) : ICashFlowConversionHelper
{
    public double ToMonthlyAmount(CashFlow cashFlow)
    {
        return cashFlow.Frequency switch
        {
            Frequency.DAILY      => cashFlow.Amount * 365 / 12,
            Frequency.WEEKLY     => cashFlow.Amount * 52 / 12,
            Frequency.BI_WEEKLY  => cashFlow.Amount * 26 / 12,
            Frequency.MONTHLY    => cashFlow.Amount,
            Frequency.QUARTERLY  => cashFlow.Amount / 3,
            Frequency.YEARLY     => cashFlow.Amount / 12,

            Frequency.ONE_TIME   => HandleOneTime(cashFlow),

            _ => HandleUnknownFrequency(cashFlow)
        };
    }

    private double HandleOneTime(CashFlow cashFlow)
    {
        logger.LogInformation(
            "Ignoring ONE_TIME cash flow {CashFlowId} ({Name}) in monthly calculation",
            cashFlow.Id,
            cashFlow.Name
        );

        return 0;
    }

    private double HandleUnknownFrequency(CashFlow cashFlow)
    {
        logger.LogWarning(
            "Unknown frequency {Frequency} for CashFlow {CashFlowId}. Amount ignored.",
            cashFlow.Frequency,
            cashFlow.Id
        );

        return 0;
    }
}