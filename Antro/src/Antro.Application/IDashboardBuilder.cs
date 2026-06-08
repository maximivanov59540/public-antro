using Antro.Domain;

namespace Antro.Application;

public interface IDashboardBuilder
{
    DashboardViewModel Build(
        IReadOnlyList<Benefit> catalog,
        UserProfile profile,
        EvaluationContext context);
}
