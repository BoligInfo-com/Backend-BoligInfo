using BoligInfo.Core.Enums;
using BoligInfo.Core.Models;

namespace BoligInfo.Core.CashFlowConversionHelper;

public class CashFlowConversionHelper
{
    private static double ToMonthlyAmount(CashFlow cf)
    {
        return cf.Frequency switch
        { 
            Frequency.MONTHLY => cf.Amount,
            Frequency.QUARTERLY => cf.Amount / 3,
            Frequency.YEARLY => cf.Amount / 12,
            Frequency.ONE_TIME => 0,
            _ => 0
        };
    }
}