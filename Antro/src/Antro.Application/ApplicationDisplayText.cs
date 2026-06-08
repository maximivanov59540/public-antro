using Antro.Domain;

namespace Antro.Application;

internal static class ApplicationDisplayText
{
    public static string GetStageTitle(LifeStage stage) =>
        stage switch
        {
            LifeStage.ExpectingChild => "Беременность",
            LifeStage.NewbornZeroToTwoMonths => "Первые два месяца после рождения",
            LifeStage.UpToSixMonths => "До шести месяцев",
            LifeStage.UpToEighteenMonths => "До полутора лет",
            LifeStage.UpToThreeYears => "До трёх лет",
            LifeStage.OlderThanThreeYears => "Старше трёх лет",
            _ => "Этап семьи"
        };

    public static string GetStageSubtitle(LifeStage stage) =>
        stage switch
        {
            LifeStage.ExpectingChild => "Показываем меры, которые важны во время ожидания ребёнка.",
            LifeStage.NewbornZeroToTwoMonths => "Сначала идут срочные и легко упускаемые меры раннего периода.",
            LifeStage.UpToSixMonths => "Показываем ранние выплаты, права и сервисы первого полугодия.",
            LifeStage.UpToEighteenMonths => "Показываем выплаты и права, актуальные в первый полтора года.",
            LifeStage.UpToThreeYears => "Показываем выплаты, права и сервисы, которые ещё могут быть полезны до трёх лет.",
            LifeStage.OlderThanThreeYears => "Показываем более поздние сервисы, налоги и долгие маршруты.",
            _ => "Показываем актуальные меры этого этапа."
        };

    public static string GetStageLabel(LifeStage stage) =>
        stage switch
        {
            LifeStage.ExpectingChild => "Беременность",
            LifeStage.NewbornZeroToTwoMonths => "0–2 месяца",
            LifeStage.UpToSixMonths => "2–6 месяцев",
            LifeStage.UpToEighteenMonths => "6–18 месяцев",
            LifeStage.UpToThreeYears => "1,5–3 года",
            LifeStage.OlderThanThreeYears => "Старше 3 лет",
            _ => "Этап семьи"
        };

    public static StatusPillViewModel GetAvailabilityPill(AvailabilityStatus status) =>
        status switch
        {
            AvailabilityStatus.Available => new("Доступно", "available"),
            AvailabilityStatus.PotentiallyAvailable => new("Может подойти", "potentially-available"),
            AvailabilityStatus.Automatic => new("Автоматически", "automatic"),
            AvailabilityStatus.Informational => new("Информация", "informational"),
            AvailabilityStatus.NeedsMoreInfo => new("Нужно уточнить", "needs-more-info"),
            AvailabilityStatus.Unavailable => new("Сейчас не подходит", "unavailable"),
            AvailabilityStatus.Expired => new("Срок прошёл", "expired"),
            _ => new("Статус", "status")
        };

    public static StatusPillViewModel GetRuleOutcomePill(RuleOutcome outcome) =>
        outcome switch
        {
            RuleOutcome.Pass => new("Условие выполнено", "condition-pass"),
            RuleOutcome.Fail => new("Условие не выполнено", "condition-fail"),
            RuleOutcome.Unknown => new("Нужно уточнить", "condition-unknown"),
            RuleOutcome.NotApplicable => new("Не влияет", "condition-not-applicable"),
            _ => new("Условие", "condition")
        };

    public static string GetConditionLabel(IEligibilityRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        return rule.Code switch
        {
            "assumed-moscow-registration" => "Московская регистрация семьи учитывается как допущение MVP.",
            "assumed-russian-citizenship" => "Российское гражданство учитывается как допущение MVP.",
            "parent-age-band" => "Возрастное условие молодой семьи должно быть выполнено.",
            "child-order" => "Очередность рождения ребёнка влияет на эту меру.",
            "income-band" => "Доход семьи должен соответствовать правилам меры.",
            "property-status" => "Жилищный статус семьи может влиять на право на меру.",
            "employment-status" => "Нужен подходящий трудовой статус родителя.",
            "matcapital-history" => "История использования маткапитала влияет на право на меру.",
            "mortgage-intent" => "Для этой меры должен быть актуален ипотечный маршрут.",
            "life-stage" => "Мера должна соответствовать текущему этапу семьи.",
            "child-age" => "Возраст ребёнка должен попадать в допустимый диапазон.",
            _ => rule.Description
        };
    }

    public static string GetIconKey(BenefitCategory category, BenefitType type) =>
        type switch
        {
            BenefitType.NaturalSupport                                   => "gift",
            BenefitType.OngoingRight or BenefitType.InformationalRight   => "shield",
            BenefitType.HousingSupport                                   => "home",
            BenefitType.TaxBenefit                                       => "document",
            _ => category switch
            {
                BenefitCategory.PregnancyAndBirth    => "pregnancy",
                BenefitCategory.Childcare            => "baby-care",
                BenefitCategory.FamilyIncomeSupport  => "money",
                BenefitCategory.Housing              => "home",
                BenefitCategory.Taxes                => "document",
                BenefitCategory.DocumentsAndServices => "briefcase",
                _                                    => "gift"
            }
        };

    public static string GetSectionCategoryKey(DeadlineStatus deadlineStatus, BenefitType type)
    {
        if (deadlineStatus is DeadlineStatus.Urgent or DeadlineStatus.Active or DeadlineStatus.Soon)
            return "urgent";
        if (type is BenefitType.OngoingRight or BenefitType.InformationalRight)
            return "rights";
        return "other";
    }

    public static string GetSectionHeaderText(string sectionKey) =>
        sectionKey switch
        {
            "urgent" => "Срочно — успейте оформить",
            "rights" => "Ваши права на этом этапе",
            _        => "Дополнительные возможности"
        };

    public static string GetSectionSubtitleText(string sectionKey) =>
        sectionKey switch
        {
            "urgent" => "У этих выплат есть дедлайн после рождения",
            "rights" => "Без жёстких сроков — оформляйте, когда удобно",
            _        => ""
        };

    public static string GetSectionColorClass(string sectionKey) =>
        sectionKey switch
        {
            "urgent" => "benefit-section--urgent",
            "rights" => "benefit-section--rights",
            _        => "benefit-section--other"
        };

    public static string GetCategoryKey(BenefitCategory category) =>
        category switch
        {
            BenefitCategory.PregnancyAndBirth => "pregnancy-and-birth",
            BenefitCategory.Childcare => "childcare",
            BenefitCategory.FamilyIncomeSupport => "family-income-support",
            BenefitCategory.Housing => "housing",
            BenefitCategory.Taxes => "taxes",
            BenefitCategory.DocumentsAndServices => "documents-and-services",
            _ => "benefits"
        };

    public static string GetLegalSourceLevelLabel(LegalSourceLevel level) =>
        level switch
        {
            LegalSourceLevel.FederalLaw => "Федеральный закон",
            LegalSourceLevel.MoscowLaw => "Закон Москвы",
            LegalSourceLevel.GovernmentResolution => "Постановление",
            LegalSourceLevel.OfficialPortal => "Официальный портал",
            _ => "Источник"
        };

    public static string GetVerificationLabel(VerificationStatus status) =>
        status switch
        {
            VerificationStatus.Draft => "Черновик",
            VerificationStatus.Checked => "Проверено",
            VerificationStatus.NeedsReview => "Нужно перепроверить",
            VerificationStatus.ConflictingSources => "Есть конфликт источников",
            _ => "Статус источника"
        };

    public static string GetQuestionKindKey(QuestionKind kind) =>
        kind switch
        {
            QuestionKind.DateInput => "date-input",
            QuestionKind.SingleSelect => "single-select",
            _ => "question"
        };
}
