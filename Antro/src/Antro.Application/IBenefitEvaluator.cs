using Antro.Domain;

namespace Antro.Application;

public interface IBenefitEvaluator
{
    IReadOnlyList<EvaluatedBenefit> Evaluate(
        IReadOnlyList<Benefit> benefits,
        UserProfile profile,
        EvaluationContext context);
}
