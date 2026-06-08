using Antro.Domain;

namespace Antro.Application;

public interface IStageShowcaseBuilder
{
    StageShowcaseViewModel Build(
        LifeStage selectedStage,
        IReadOnlyList<Benefit> catalog,
        EvaluationContext context,
        UserProfile? profile = null);
}
