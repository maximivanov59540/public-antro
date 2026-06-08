using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static partial class Moscow2026BenefitCatalog
{
    public static Benefit PregnancyMonthlyBenefit { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.PregnancyMonthlyBenefit,
        "pregnancy-monthly-benefit",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier1Core,
        BenefitCategory.FamilyIncomeSupport,
        Moscow2026DraftContent.Copy(
            "Ежемесячная выплата при беременности",
            "Единое пособие беременной женщине",
            "Ежемесячная выплата для беременной женщины при соблюдении доходных и иных критериев.",
            "Единое пособие беременной женщине: 50 / 75 / 100% ПМ трудоспособного населения региона. Для Москвы 2026 — 14 470 / 21 705 / 28 940 ₽. Условие — постановка на учёт до 12 недель и доход ниже регионального ПМ на душу (income-правило ещё уточняется)."),
        new AmountRange(
            new MoneyAmount(14_470m, Currency.Rub, AmountPeriod.Monthly),
            new MoneyAmount(28_940m, Currency.Rub, AmountPeriod.Monthly)),
        new NoDeadline("Draft: the benefit is tied to pregnancy timing and current criteria rather than a simple birth-based filing deadline."),
        Moscow2026StageAvailability.PregnancyWithEarlyPostpartumContext("Shown after birth only as context for a pregnancy-stage payment that has ended."),
        Moscow2026LegalSources.FederalPortal("Госуслуги/СФР: единое пособие беременной женщине", "gosuslugi-unified-pregnancy-benefit", "50/75/100% ПМ трудоспособного (Москва: 14 470 / 21 705 / 28 940 ₽); доход ниже 1 ПМ на душу; постановка на учёт до 12 недель.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Gosuslugi or SFR", "Income and household composition proof"),
        Moscow2026DraftContent.PortalApplicationSteps("Gosuslugi or SFR"),
        eligibilityRules: Moscow2026EligibilityRules.PregnancyMonthlyBenefit());

    public static Benefit MaternityLeaveBenefit { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MaternityLeaveBenefit,
        "maternity-leave-benefit",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier1Core,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Декретные выплаты",
            "Пособие по беременности и родам",
            "Выплата за весь период отпуска по беременности и родам; зависит от заработка и длительности отпуска (140 / 156 / 194 дня).",
            "Пособие по беременности и родам зависит от заработка и длительности отпуска. Минимум/максимум за весь отпуск (2026): обычные роды, 140 дней — 124 702,20–955 836 ₽; осложнённые роды, 156 дней — 138 953,88–1 065 074,40 ₽; многоплодная беременность, 194 дня — 172 801,62–1 324 515,60 ₽. Срок обращения — не позднее 6 месяцев со дня окончания отпуска по беременности и родам."),
        new StatusOnlyAmount("Сумма за весь отпуск зависит от сценария: 140 дней — 124 702–955 836 ₽; 156 дней — 138 954–1 065 074 ₽; 194 дня — 172 802–1 324 516 ₽."),
        new NoDeadline("Срок обращения — не позднее 6 месяцев со дня окончания отпуска по беременности и родам (не от рождения)."),
        Moscow2026StageAvailability.PregnancyWithEarlyPostpartumContext("Shown briefly after birth because the payment relates to the maternity-leave period around childbirth."),
        Moscow2026LegalSources.FederalLaw("Пособие по беременности и родам", "255-ФЗ", "Сценарии 140/156/194 дня с минимум/максимумом (СФР); срок обращения 6 месяцев со дня окончания отпуска.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Employer or SFR", "Sick-leave certificate or maternity leave document"),
        Moscow2026DraftContent.PortalApplicationSteps("Employer, SFR, or Gosuslugi"),
        eligibilityRules: Moscow2026EligibilityRules.MaternityLeaveBenefit());

    public static Benefit MoscowEarlyPregnancyRegistrationPayment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MoscowEarlyPregnancyRegistrationPayment,
        "moscow-early-pregnancy-registration-payment",
        BenefitType.DeadlineEvent,
        BenefitTier.Tier2Value,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Московская выплата за раннюю постановку на учёт",
            "Московская выплата беременной женщине за раннюю постановку на учёт",
            "Разовая московская выплата женщине, вставшей на учёт в медорганизации Москвы до 20 недель беременности.",
            "Единовременная выплата города Москвы женщине, вставшей на учёт в медицинской организации, действующей на территории Москвы, в срок до 20 недель беременности. На 2026 год — 892 ₽. Точный срок обращения и набор документов уточняются по странице услуги mos.ru, поэтому дедлайн пока не фиксируется."),
        new FixedAmount(new MoneyAmount(892m, Currency.Rub, AmountPeriod.OneTime)),
        new NoDeadline("Сумма подтверждена (892 ₽, 2026), но точный срок обращения требует отдельной выгрузки страницы услуги mos.ru."),
        Moscow2026StageAvailability.PregnancyOnly("Shown after birth only as an expired pregnancy-stage payment."),
        Moscow2026LegalSources.MoscowPortal("mos.ru: выплата за раннюю постановку на учёт до 20 недель", "mosru-early-pregnancy-registration-payment", "Сумма 892 ₽ (2026) подтверждена; срок обращения требует проверки страницы услуги.", verificationStatus: VerificationStatus.NeedsReview),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru or MFC", "Medical certificate about early registration"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru or MFC"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());

    public static Benefit PregnancyMilkKitchen { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.PregnancyMilkKitchen,
        "pregnancy-milk-kitchen",
        BenefitType.NaturalSupport,
        BenefitTier.Tier2Value,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Бесплатные продукты для беременных — молочная кухня",
            "Обеспечение беременных женщин продуктами питания",
            "Натуральная поддержка в виде продуктов для беременной женщины по московским правилам.",
            "Это натуральная поддержка (продукты), а не денежная выплата. Маршрут — через врача и mos.ru. Конкретный ассортимент, нормы выдачи и список документов устанавливаются приказом ДЗМ и не финализируются здесь без выгрузки актуального приказа."),
        new NaturalSupport("Product support through the Moscow milk-kitchen system."),
        new NoDeadline("Draft: issuance timing and prescription rules depend on the current medical route rather than a simple filing deadline."),
        Moscow2026StageAvailability.PregnancyOnly("Shown after birth only as an expired pregnancy-stage support measure."),
        Moscow2026LegalSources.MoscowPortal("mos.ru: молочная кухня для беременных", "mosru-pregnancy-milk-kitchen", "Маршрут подтверждён; ассортимент и нормы выдачи — HOLD до выгрузки приказа ДЗМ.", verificationStatus: VerificationStatus.NeedsReview),
        Moscow2026DraftContent.ApplicationDocuments("Clinic or mos.ru", "Medical prescription or clinic referral"),
        Moscow2026DraftContent.PortalApplicationSteps("clinic, mos.ru, or the listed distribution point"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());

    public static Benefit PregnancyDismissalProtection { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.PregnancyDismissalProtection,
        "pregnancy-dismissal-protection",
        BenefitType.OngoingRight,
        BenefitTier.Tier1Core,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Дополнительные гарантии при увольнении",
            "Гарантии беременной работнице при увольнении (ст. 261 ТК РФ)",
            "Беременную работницу, как правило, нельзя уволить по инициативе работодателя — но есть исключения.",
            "По общему правилу расторжение трудового договора по инициативе работодателя с беременной женщиной не допускается. Исключения: ликвидация организации или прекращение деятельности ИП, а также отдельные виновные основания, прямо предусмотренные законом. Срочный трудовой договор по заявлению работницы и при наличии медицинской справки продлевается до окончания беременности. Это трудовая гарантия, а не денежная выплата."),
        new StatusOnlyAmount("Трудовая гарантия при увольнении; прямой денежной суммы нет."),
        new NoDeadline("The right is ongoing during pregnancy rather than tied to a separate filing deadline."),
        Moscow2026StageAvailability.PregnancyOnly("Shown after birth only as an expired pregnancy-stage employment protection."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: гарантии беременным при увольнении", "ст. 261", "Запрет увольнения по инициативе работодателя с исключениями (ликвидация/прекращение ИП, виновные основания).", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Pregnancy certificate or medical proof"),
        Moscow2026DraftContent.EmployerRightSteps("dismissal protection right"),
        eligibilityRules: Moscow2026EligibilityRules.PregnancyEmploymentRight());

    public static Benefit PregnancyLightWorkRight { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.PregnancyLightWorkRight,
        "pregnancy-light-work-right",
        BenefitType.OngoingRight,
        BenefitTier.Tier2Value,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Право на лёгкий труд и ограничения по работе",
            "Перевод беременной работницы на лёгкий труд (ст. 254 ТК РФ)",
            "По медицинскому заключению и заявлению беременной снижают нормы выработки/обслуживания или переводят на работу без вредных факторов — со средним заработком.",
            "На основании медицинского заключения и заявления работницы работодатель снижает нормы выработки или обслуживания либо переводит беременную на другую работу без воздействия неблагоприятных производственных факторов. При этом сохраняется средний заработок по прежней работе. Это трудовая гарантия, а не отдельная денежная выплата; действует на период беременности до предоставления подходящей работы."),
        new StatusOnlyAmount("Сохраняется средний заработок при переводе/снижении норм; отдельной выплаты-суммы у этого права нет."),
        new NoDeadline("The right is ongoing while the pregnancy and medical restrictions remain relevant."),
        Moscow2026StageAvailability.PregnancyOnly("Shown after birth only as an expired pregnancy-stage labor right."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: лёгкий труд для беременной работницы", "ст. 254", "Нужны медицинское заключение и заявление; сохраняется средний заработок.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Medical recommendation for work restrictions"),
        Moscow2026DraftContent.EmployerRightSteps("light-work right"),
        eligibilityRules: Moscow2026EligibilityRules.PregnancyEmploymentRight());

    public static Benefit PregnancyPartTimeRight { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.PregnancyPartTimeRight,
        "pregnancy-part-time-right",
        BenefitType.OngoingRight,
        BenefitTier.Tier2Value,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Право на неполный рабочий день",
            "Право на неполное рабочее время (ст. 93 ТК РФ)",
            "Работодатель обязан установить неполное время по просьбе беременной, а также родителя/опекуна ребёнка до 14 лет (ребёнка-инвалида до 18) и лиц, ухаживающих за больным членом семьи.",
            "Неполный рабочий день или неделю работодатель обязан установить по просьбе: беременной женщины; одного из родителей (опекуна, попечителя) ребёнка до 14 лет или ребёнка-инвалида до 18 лет; лица, ухаживающего за больным членом семьи по медицинскому заключению. Оплата — пропорционально отработанному времени или объёму работ; отпуск и стаж по самому факту неполного времени не урезаются. Срок устанавливается по обстоятельству или соглашению, но не дольше периода наличия основания."),
        new StatusOnlyAmount("Оплата пропорциональна отработанному времени; отдельной выплаты-суммы у этого права нет."),
        new NoDeadline("The right is ongoing while the pregnancy-related need for part-time work remains relevant."),
        Moscow2026StageAvailability.PregnancyOnly("Shown after birth only as an expired pregnancy-stage work-format right."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: неполное рабочее время", "ст. 93", "Общее право: беременная, родитель/опекун ребёнка до 14 (инвалида до 18), уход за больным членом семьи.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Written request to employer and pregnancy proof"),
        Moscow2026DraftContent.EmployerRightSteps("part-time work right"),
        eligibilityRules: Moscow2026EligibilityRules.PregnancyEmploymentRight());
}
