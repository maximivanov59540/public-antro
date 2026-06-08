using Antro.Domain;

namespace Antro.Application;

public interface IQuestionnaireFlow
{
    IReadOnlyList<QuestionnaireStep> Steps { get; }

    IReadOnlyList<QuestionnaireStep> GetVisibleSteps(UserProfileDraft draft);

    UserProfile BuildProfile(UserProfileDraft draft);
}
