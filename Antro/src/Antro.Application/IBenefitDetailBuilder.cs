using Antro.Domain;

namespace Antro.Application;

public interface IBenefitDetailBuilder
{
    BenefitDetailViewModel Build(
        Benefit benefit,
        UserProfile profile,
        EvaluationContext context);
}
