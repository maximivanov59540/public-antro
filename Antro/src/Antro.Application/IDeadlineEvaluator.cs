using Antro.Domain;

namespace Antro.Application;

public interface IDeadlineEvaluator
{
    DeadlineEvaluation Evaluate(DeadlineRule? deadlineRule, UserProfile profile, EvaluationContext context);
}
