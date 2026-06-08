using Antro.Domain;

namespace Antro.Application;

public interface IAmountEvaluator
{
    MoneyEstimate Evaluate(AmountRule amountRule, UserProfile profile, EvaluationContext context);
}
