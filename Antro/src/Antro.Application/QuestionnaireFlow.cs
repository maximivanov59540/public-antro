using Antro.Domain;

namespace Antro.Application;

public enum QuestionnaireStepId
{
    ChildDate = 0,
    ChildOrder = 1,
    MatCapitalHistory = 2,
    FamilyStatus = 3,
    EmploymentStatus = 4,
    ParentAgeBand = 5,
    IncomeBand = 6,
    PropertyStatus = 7,
    MortgageIntent = 8
}

public enum QuestionKind
{
    DateInput = 0,
    SingleSelect = 1
}

public sealed record QuestionOption(
    string Value,
    string Label,
    string? Description = null,
    string? IconName = null,
    string? Glyph = null);

public sealed record QuestionnaireStep(
    QuestionnaireStepId Id,
    string Title,
    string Prompt,
    QuestionKind Kind,
    IReadOnlyList<QuestionOption> Options,
    string? HelpText = null,
    Func<UserProfileDraft, bool>? ShouldShow = null,
    string Layout = "list")
{
    public bool IsVisible(UserProfileDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        return ShouldShow?.Invoke(draft) ?? true;
    }
}

public sealed record UserProfileDraft
{
    public DateOnly? ChildDate { get; init; }

    public DateInputKind DateInputKind { get; init; } = DateInputKind.Unknown;

    public ChildOrder? ChildOrder { get; init; }

    public MatCapitalHistory? MatCapitalHistory { get; init; }

    public FamilyStatus? FamilyStatus { get; init; }

    public EmploymentStatus? EmploymentStatus { get; init; }

    public ParentAgeBand? ParentAgeBand { get; init; }

    public IncomeBand? IncomeBand { get; init; }

    public PropertyStatus? PropertyStatus { get; init; }

    public MortgageIntent? MortgageIntent { get; init; }
}

public sealed class QuestionnaireFlow : IQuestionnaireFlow
{
    public IReadOnlyList<QuestionnaireStep> Steps { get; } =
    [
        new(
            QuestionnaireStepId.ChildDate,
            "Когда родился ваш малыш?",
            "Мы рассчитаем точные сроки для каждой выплаты",
            QuestionKind.DateInput,
            [
                new QuestionOption(DateInputKind.BirthDate.ToString(), "Дата рождения"),
                new QuestionOption(DateInputKind.DueDate.ToString(), "Ожидаемая дата родов")
            ]),
        new(
            QuestionnaireStepId.ChildOrder,
            "Какой это ребёнок?",
            "Влияет на размер маткапитала и некоторых выплат",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(ChildOrder.First.ToString(), "Первый ребёнок", Glyph: "1"),
                new QuestionOption(ChildOrder.Second.ToString(), "Второй ребёнок", Glyph: "2")
            ]),
        new(
            QuestionnaireStepId.MatCapitalHistory,
            "Получали маткапитал раньше?",
            "Это влияет на сумму: <b>963 200 ₽</b> или доплата <b>234 300 ₽</b>",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(MatCapitalHistory.NeverReceived.ToString(), "Нет, не получали"),
                new QuestionOption(MatCapitalHistory.ReceivedUnused.ToString(), "Да, получали на первого ребёнка")
            ],
            ShouldShow: draft => draft.ChildOrder == ChildOrder.Second),
        new(
            QuestionnaireStepId.FamilyStatus,
            "Ваш семейный статус",
            "Нужно для расчёта дохода семьи",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(FamilyStatus.Married.ToString(), "В браке", IconName: "rings"),
                new QuestionOption(FamilyStatus.Partners.ToString(), "Не в браке, но оба родителя участвуют", IconName: "pair"),
                new QuestionOption(FamilyStatus.SingleParent.ToString(), "Один родитель", IconName: "person")
            ]),
        new(
            QuestionnaireStepId.EmploymentStatus,
            "Кто работает официально?",
            "Влияет на декретные, вычеты и трудовые гарантии",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(EmploymentStatus.MotherOnly.ToString(), "Мама", IconName: "female"),
                new QuestionOption(EmploymentStatus.FatherOnly.ToString(), "Папа", IconName: "male"),
                new QuestionOption(EmploymentStatus.Both.ToString(), "Оба", IconName: "pair"),
                new QuestionOption(EmploymentStatus.NeitherParent.ToString(), "Никто", IconName: "neutral")
            ],
            Layout: "grid"),
        new(
            QuestionnaireStepId.ParentAgeBand,
            "Обоим родителям меньше 36 лет?",
            "Молодые семьи в Москве получают дополнительную выплату <b>до 177 000 ₽</b>",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(ParentAgeBand.BothUnderThirtySix.ToString(), "Да, обоим меньше 36"),
                new QuestionOption(ParentAgeBand.AtLeastOneThirtySixOrOlder.ToString(), "Нет, хотя бы одному 36 или больше"),
                new QuestionOption(ParentAgeBand.Mixed.ToString(), "Одному — да, другому — нет")
            ]),
        new(
            QuestionnaireStepId.IncomeBand,
            "Примерный доход на человека в семье?",
            "Не точная сумма — просто диапазон. Влияет на <b>3 крупные выплаты</b>.",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(IncomeBand.UpToOneLivingMinimum.ToString(), "До 25 342 ₽",
                    Description: "<span class=\"good\">✓</span> Подходит для единого пособия"),
                new QuestionOption(IncomeBand.UpToOneAndHalfLivingMinimum.ToString(), "25 343 – 38 013 ₽",
                    Description: "<span class=\"good\">✓</span> Подходит для семейной выплаты"),
                new QuestionOption(IncomeBand.UpToTwoLivingMinimums.ToString(), "38 014 – 50 684 ₽",
                    Description: "<span class=\"good\">✓</span> Подходит для выплаты из маткапитала"),
                new QuestionOption(IncomeBand.AboveTwoLivingMinimums.ToString(), "Выше 50 684 ₽",
                    Description: "<span class=\"bad\">✕</span> Основные выплаты по доходу недоступны"),
                new QuestionOption(IncomeBand.Unknown.ToString(), "Не знаю, помогите разобраться",
                    IconName: "q")
            ]),
        new(
            QuestionnaireStepId.PropertyStatus,
            "Есть ли у семьи крупное имущество?",
            "Несколько квартир, машин или крупные вклады. Влияет на единое пособие.",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(PropertyStatus.DoesNotOwnHome.ToString(), "Нет, обычный набор", IconName: "check"),
                new QuestionOption(PropertyStatus.OwnsHome.ToString(), "Да, есть", IconName: "nope"),
                new QuestionOption(PropertyStatus.Unsure.ToString(), "Не уверены", IconName: "q")
            ]),
        new(
            QuestionnaireStepId.MortgageIntent,
            "Планируете покупать жильё или брать ипотеку?",
            "Семьям с детьми доступна <b>ипотека под 6%</b> и маткапитал на жильё",
            QuestionKind.SingleSelect,
            [
                new QuestionOption(MortgageIntent.PlansToUseMortgage.ToString(), "Да, планируем", IconName: "house"),
                new QuestionOption(MortgageIntent.None.ToString(), "Нет, пока не планируем", IconName: "neutral")
            ])
    ];

    public IReadOnlyList<QuestionnaireStep> GetVisibleSteps(UserProfileDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        return Steps.Where(step => step.IsVisible(draft)).ToArray();
    }

    public UserProfile BuildProfile(UserProfileDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return new UserProfile
        {
            ChildDate = draft.ChildDate,
            DateInputKind = draft.DateInputKind,
            ChildOrder = draft.ChildOrder,
            MatCapitalHistory = draft.ChildOrder == ChildOrder.Second ? draft.MatCapitalHistory : null,
            FamilyStatus = draft.FamilyStatus,
            EmploymentStatus = draft.EmploymentStatus,
            ParentAgeBand = draft.ParentAgeBand,
            IncomeBand = draft.IncomeBand,
            PropertyStatus = draft.PropertyStatus,
            MortgageIntent = draft.MortgageIntent
        };
    }
}
