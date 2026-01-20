using BoligInfo.Core.Models;

namespace BoligInfo.BusinessLogic.CashFlowConversionHelper;

public interface ICashFlowConversionHelper
{
    public double ToMonthlyAmount(CashFlow cf);
}