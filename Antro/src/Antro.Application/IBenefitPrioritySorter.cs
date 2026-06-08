using Antro.Domain;

namespace Antro.Application;

public interface IBenefitPrioritySorter
{
    IReadOnlyList<EvaluatedBenefit> Sort(IReadOnlyList<EvaluatedBenefit> benefits);
}
