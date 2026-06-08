using Antro.Application;
using Antro.Domain;

namespace Antro.Application.Tests;

public sealed class QuestionnaireFlowTests
{
    private readonly IQuestionnaireFlow flow = new QuestionnaireFlow();

    [Fact]
    public void Questionnaire_definition_has_nine_logical_questions()
    {
        Assert.Equal(9, flow.Steps.Count);
    }

    [Fact]
    public void First_child_skips_matcapital_history_question()
    {
        var visibleSteps = flow.GetVisibleSteps(
            new UserProfileDraft
            {
                ChildOrder = ChildOrder.First
            });

        Assert.DoesNotContain(visibleSteps, step => step.Id == QuestionnaireStepId.MatCapitalHistory);
    }

    [Fact]
    public void Second_child_shows_matcapital_history_question()
    {
        var visibleSteps = flow.GetVisibleSteps(
            new UserProfileDraft
            {
                ChildOrder = ChildOrder.Second
            });

        Assert.Contains(visibleSteps, step => step.Id == QuestionnaireStepId.MatCapitalHistory);
    }

    [Fact]
    public void Unknown_income_maps_to_income_band_unknown()
    {
        var profile = flow.BuildProfile(
            new UserProfileDraft
            {
                IncomeBand = IncomeBand.Unknown
            });

        Assert.Equal(IncomeBand.Unknown, profile.IncomeBand);
    }
}
