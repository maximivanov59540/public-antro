using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static partial class Moscow2026BenefitCatalog
{
    public static Benefit MoscowNewbornGiftSet { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MoscowNewbornGiftSet,
        "moscow-newborn-gift-set",
        BenefitType.DeadlineEvent,
        BenefitTier.Tier2Value,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Подарочный набор «Наше сокровище»",
            "Подарочный комплект для новорождённого «Наше сокровище»",
            "Натуральная московская поддержка после рождения ребёнка: комплект из 50 предметов или денежная компенсация 20 000 ₽.",
            "Подарочный комплект для новорождённого («Наше сокровище») — около 50 предметов; вместо него можно выбрать денежную компенсацию 20 000 ₽. Маршруты: в роддоме при выписке; после выписки по заявлению; либо денежная альтернатива. Также есть отдельные сценарии для детей, родившихся вне московского роддома или в другом регионе, если выполняются московские условия. Заявление (после выписки или на компенсацию) — не позднее 2 месяцев со дня рождения."),
        new NaturalSupport("Комплект из ~50 предметов для новорождённого; альтернатива — денежная компенсация 20 000 ₽."),
        new FilingDeadlineFromBirth(2, "После выписки или при выборе компенсации заявление подаётся не позднее 2 месяцев со дня рождения."),
        Moscow2026StageAvailability.NewbornTwoMonthDeadline("The ordinary filing window is treated as expired after the first two months from birth."),
        Moscow2026LegalSources.MoscowPortal("ДТСЗН: приоритетный проект «Наше сокровище»", "mosru-our-treasure-gift-set", "Комплект ~50 предметов или 20 000 ₽; маршруты роддом / после выписки / компенсация; срок 2 месяца.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru, MFC, or maternity hospital route", "Child birth certificate or maternity-hospital issue document"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru, MFC, or the maternity-hospital route"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly(),
        priorityHint: Moscow2026PriorityHints.NewbornGiftSet);

    public static Benefit FederalBirthGrant { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.FederalBirthGrant,
        "federal-birth-grant",
        BenefitType.DeadlineEvent,
        BenefitTier.Tier1Core,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Федеральное пособие при рождении",
            "Единовременное пособие при рождении ребёнка",
            "Федеральная разовая выплата после рождения ребёнка.",
            "Федеральное единовременное пособие при рождении ребёнка. С 01.02.2026 — 28 450,45 ₽; районный коэффициент применяется там, где он установлен. Выплачивается на каждого ребёнка."),
        new FixedAmount(new MoneyAmount(28_450.45m, Currency.Rub, AmountPeriod.OneTime)),
        new FilingDeadlineFromBirth(6, "Для неработающих заявление подаётся в течение 6 месяцев со дня рождения ребёнка; назначение и выплата — по правилам СФР."),
        Moscow2026StageAvailability.NewbornSixMonthDeadline("The ordinary filing window is treated as expired after six months from birth."),
        Moscow2026LegalSources.FederalLaw("Единовременное пособие при рождении ребёнка", "81-ФЗ", "Сумма 28 450,45 ₽ с 01.02.2026 (СФР). Заменяет прежние 26 941 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Gosuslugi, SFR, or employer", "Birth certificate and applicant status proof"),
        Moscow2026DraftContent.PortalApplicationSteps("Gosuslugi, SFR, or employer"),
        eligibilityRules: Moscow2026EligibilityRules.FederalBirthGrant(),
        priorityHint: Moscow2026PriorityHints.BirthDeadlinePayment);

    public static Benefit MoscowBirthPayment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MoscowBirthPayment,
        "moscow-birth-payment",
        BenefitType.DeadlineEvent,
        BenefitTier.Tier1Core,
        BenefitCategory.PregnancyAndBirth,
        Moscow2026DraftContent.Copy(
            "Московская выплата при рождении",
            "Единовременная компенсационная выплата при рождении ребёнка",
            "Московская разовая выплата после рождения ребёнка.",
            "Единовременная компенсационная выплата города Москвы на возмещение расходов в связи с рождением ребёнка. Размер зависит от очерёдности рождения; не путать с федеральным пособием при рождении."),
        new ChoiceAmount(
        [
            new ChoiceAmountOption(new MoneyAmount(8_157m, Currency.Rub, AmountPeriod.OneTime), label: "На первого ребёнка", childOrder: ChildOrder.First),
            new ChoiceAmountOption(new MoneyAmount(21_498m, Currency.Rub, AmountPeriod.OneTime), label: "На второго и последующего ребёнка", childOrder: ChildOrder.Second),
            new ChoiceAmountOption(new MoneyAmount(21_498m, Currency.Rub, AmountPeriod.OneTime), label: "На второго и последующего ребёнка", childOrder: ChildOrder.ThirdOrLater)
        ],
        "Суммы 2026 года. При одновременном рождении (усыновлении) трёх и более детей выплата составляет 74 121 ₽. При мертворождении не назначается."),
        new FilingDeadlineFromBirth(6, "По материалам ДТСЗН срок обращения — не позднее 6 месяцев со дня рождения ребёнка."),
        Moscow2026StageAvailability.NewbornSixMonthDeadline("The ordinary filing window is treated as expired after six months from birth."),
        Moscow2026LegalSources.MoscowLaw("Единовременная компенсационная выплата при рождении ребёнка", "Закон Москвы № 60; ПП Москвы от 09.12.2025 № 3025-ПП", "Суммы 2026: 8 157 / 21 498 / 74 121 ₽. Перед публичным релизом желательно закрепить актуальный текст услуги mos.ru.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru or MFC", "Moscow registration proof"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru or MFC"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly(),
        priorityHint: Moscow2026PriorityHints.BirthDeadlinePayment);

    public static Benefit MoscowYoungFamilyPayment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MoscowYoungFamilyPayment,
        "young-family-payment",
        BenefitType.DeadlineEvent,
        BenefitTier.Tier1Core,
        BenefitCategory.FamilyIncomeSupport,
        Moscow2026DraftContent.Copy(
            "Выплата молодой семье",
            "Дополнительное единовременное пособие молодой семье в связи с рождением ребёнка",
            "Московская разовая выплата молодой семье при рождении ребёнка.",
            "Разовая московская выплата для молодой семьи после рождения ребёнка. В MVP карточка показывает сумму по очередности рождения ребёнка, проверяет возрастное условие для родителя и напоминает о годовом сроке подачи. Точный нормативный акт и финальная формулировка условий ещё помечены как черновик и требуют перепроверки."),
        new ChoiceAmount(
        [
            new ChoiceAmountOption(new MoneyAmount(126_710m, Currency.Rub, AmountPeriod.OneTime), label: "На первого ребёнка", childOrder: ChildOrder.First),
            new ChoiceAmountOption(new MoneyAmount(177_394m, Currency.Rub, AmountPeriod.OneTime), label: "На второго ребёнка", childOrder: ChildOrder.Second),
            new ChoiceAmountOption(new MoneyAmount(253_420m, Currency.Rub, AmountPeriod.OneTime), label: "На третьего или последующего ребёнка", childOrder: ChildOrder.ThirdOrLater)
        ],
        "Суммы 2026 года по кратности московского ПМ на душу населения (5 / 7 / 10 × 25 342 ₽)."),
        new FilingDeadlineFromBirth(12, "Срок обращения — 12 месяцев со дня рождения ребёнка; срок предоставления услуги на mos.ru — 10 рабочих дней."),
        Moscow2026StageAvailability.NewbornTwelveMonthDeadline("This stage covers both active months before the first birthday and expired months after it, so the benefit stays visible as stage-dependent."),
        [
            new LegalSource(
                "mos.ru: дополнительное единовременное пособие молодой семье",
                "mosru-young-family-payment",
                null,
                LegalSourceLevel.OfficialPortal,
                Moscow2026LegalSources.RegisterCheckDate,
                VerificationStatus.Checked,
                "Условия молодой семьи и срок услуги; суммы рассчитаны через ПМ Москвы 2026."),
            new LegalSource(
                "Закон города Москвы «О молодёжи»; ПП Москвы № 199-ПП (ред. № 718-ПП)",
                "moscow-law-39-young-family",
                null,
                LegalSourceLevel.MoscowLaw,
                Moscow2026LegalSources.RegisterCheckDate,
                VerificationStatus.Checked,
                "Базовый нормативный контур московской дополнительной выплаты молодой семье.")
        ],
        [
            new DocumentRequirement(
                "Паспорт заявителя",
                "Нужен документ, удостоверяющий личность заявителя.",
                "Обычно вместе с ним потребуется подтверждение регистрации в Москве или отдельный документ о регистрации."),
            new DocumentRequirement(
                "Свидетельство о рождении ребёнка",
                "Подтверждает рождение ребёнка и дату, от которой считается 12-месячный срок.",
                null),
            new DocumentRequirement(
                "Подтверждение возраста родителей",
                "Нужны документы, из которых видно, что на дату рождения ребёнка родителю или обоим родителям не исполнилось 36 лет.",
                "Точную формулировку возрастного условия нужно перепроверить по официальной инструкции."),
            new DocumentRequirement(
                "Подтверждение московской регистрации семьи и ребёнка",
                "Подойдут документы, которые использует текущий маршрут mos.ru или МФЦ.",
                "Точный набор документов для регистрации ребёнка и второго родителя в MVP пока помечен как черновик.")
        ],
        [
            new ActionStep(
                1,
                "Проверьте срок и возрастное условие",
                "Убедитесь, что после рождения ребёнка прошло не больше 12 месяцев и что возрастное условие молодой семьи выполняется.",
                "Если ребёнок уже родился давно или родителям исполнилось 36+, сначала перепроверьте официальные исключения."),
            new ActionStep(
                2,
                "Соберите документы по рождению и регистрации",
                "Подготовьте свидетельство о рождении, паспорта и подтверждение московской регистрации семьи.",
                "При подаче могут дополнительно попросить подтверждение регистрации ребёнка или второго родителя."),
            new ActionStep(
                3,
                "Подайте заявление через mos.ru или МФЦ",
                "Откройте актуальный маршрут на mos.ru или уточните очную подачу в МФЦ и отправьте комплект документов.",
                "MVP не фиксирует один обязательный канал, потому что маршрут нужно перепроверять по официальной инструкции."),
            new ActionStep(
                4,
                "Отследите решение и дозапросы",
                "После подачи проверяйте уведомления и быстро отвечайте на возможные запросы о дополнительных документах.",
                "Если ведомство просит уточнить возраст, регистрацию или состав семьи, сверяйте запрос с официальным описанием меры.")
        ],
        eligibilityRules: Moscow2026EligibilityRules.MoscowYoungFamilyPayment(),
        priorityHint: Moscow2026PriorityHints.YoungFamilyDeadlinePayment);

    public static Benefit MaternityCapital { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MaternityCapital,
        "maternity-capital",
        BenefitType.Automatic,
        BenefitTier.Tier1Core,
        BenefitCategory.FamilyIncomeSupport,
        Moscow2026DraftContent.Copy(
            "Материнский капитал",
            "Материнский (семейный) капитал",
            "Федеральный сертификат поддержки семьи после рождения детей.",
            "Сертификат обычно оформляется проактивно по данным ЗАГС/СФР. Размер с 01.02.2026 зависит от очерёдности рождения и истории использования права; остаток сертификата индексируется."),
        new ChoiceAmount(
        [
            new ChoiceAmountOption(
                new MoneyAmount(728_921.90m, Currency.Rub, AmountPeriod.OneTime),
                label: "First child, capital not received before",
                childOrder: ChildOrder.First,
                matCapitalHistory: MatCapitalHistory.NeverReceived),
            new ChoiceAmountOption(
                new MoneyAmount(963_243.17m, Currency.Rub, AmountPeriod.OneTime),
                label: "Second or later child, capital not received before",
                childOrder: ChildOrder.Second,
                matCapitalHistory: MatCapitalHistory.NeverReceived),
            new ChoiceAmountOption(
                new MoneyAmount(963_243.17m, Currency.Rub, AmountPeriod.OneTime),
                label: "Third or later child, capital not received before",
                childOrder: ChildOrder.ThirdOrLater,
                matCapitalHistory: MatCapitalHistory.NeverReceived),
            new ChoiceAmountOption(
                new MoneyAmount(234_321.27m, Currency.Rub, AmountPeriod.OneTime),
                label: "Additional amount after earlier certificate",
                childOrder: ChildOrder.Second,
                matCapitalHistory: MatCapitalHistory.ReceivedUnused),
            new ChoiceAmountOption(
                new MoneyAmount(234_321.27m, Currency.Rub, AmountPeriod.OneTime),
                label: "Additional amount after earlier certificate",
                childOrder: ChildOrder.Second,
                matCapitalHistory: MatCapitalHistory.PartiallyUsed),
            new ChoiceAmountOption(
                new MoneyAmount(234_321.27m, Currency.Rub, AmountPeriod.OneTime),
                label: "Additional amount after earlier certificate",
                childOrder: ChildOrder.ThirdOrLater,
                matCapitalHistory: MatCapitalHistory.ReceivedUnused),
            new ChoiceAmountOption(
                new MoneyAmount(234_321.27m, Currency.Rub, AmountPeriod.OneTime),
                label: "Additional amount after earlier certificate",
                childOrder: ChildOrder.ThirdOrLater,
                matCapitalHistory: MatCapitalHistory.PartiallyUsed)
        ],
        "Суммы с 01.02.2026 (СФР): 728 921,90 ₽ — первый ребёнок / базовый сертификат; 963 243,17 ₽ — второй и последующий, если право ранее не возникало; 234 321,27 ₽ — доплата после сертификата на первого ребёнка."),
        new NoDeadline("The MVP treats matcapital as automatic and without an ordinary filing deadline."),
        Moscow2026StageAvailability.PostBirthAllStages("Still relevant later because the certificate can be used after the newborn period."),
        Moscow2026LegalSources.FederalLaw("Материнский (семейный) капитал", "256-ФЗ", "Размер сертификата и доплаты с 01.02.2026 (СФР). Заменяет прежние 690 267 / 912 162 / 221 895 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("SFR or Gosuslugi", "Child-order and prior-use information"),
        Moscow2026DraftContent.PortalApplicationSteps("SFR or Gosuslugi"),
        eligibilityRules: Moscow2026EligibilityRules.MaternityCapital());

    public static Benefit ChildCareMonthlyAllowance { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.ChildCareMonthlyAllowance,
        "child-care-monthly-allowance",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier1Core,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Ежемесячное пособие по уходу",
            "Ежемесячное пособие по уходу за ребёнком",
            "Ежемесячная выплата по уходу за ребёнком в раннем возрасте.",
            "Ежемесячное пособие по уходу за ребёнком до 1,5 лет — 40% среднего заработка. С 01.02.2026 минимум 10 669,64 ₽/мес. (без районного коэффициента), максимум 83 021,18 ₽/мес."),
        new AmountRange(
            new MoneyAmount(10_669.64m, Currency.Rub, AmountPeriod.Monthly),
            new MoneyAmount(83_021.18m, Currency.Rub, AmountPeriod.Monthly)),
        new ActiveUntilChildAge(18, "The MVP treats the monthly childcare allowance as active until the child reaches 1.5 years."),
        Moscow2026StageAvailability.UntilEighteenMonths("The ordinary payment period is treated as expired after the child reaches 1.5 years."),
        Moscow2026LegalSources.FederalLaw("Ежемесячное пособие по уходу за ребёнком до 1,5 лет", "255-ФЗ", "Минимум/максимум с 01.02.2026 (СФР). Заменяет прежние 10 103 / 68 995 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Employer, SFR, or Gosuslugi", "Employment and earnings proof"),
        Moscow2026DraftContent.PortalApplicationSteps("Employer, SFR, or Gosuslugi"),
        eligibilityRules: Moscow2026EligibilityRules.ChildCareMonthlyAllowance());

    public static Benefit UnifiedChildBenefit { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.UnifiedChildBenefit,
        "unified-child-benefit",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier1Core,
        BenefitCategory.FamilyIncomeSupport,
        Moscow2026DraftContent.Copy(
            "Единое пособие на ребёнка",
            "Единое пособие на ребёнка",
            "Ежемесячная выплата на ребёнка при соблюдении доходных и имущественных критериев.",
            "Единое пособие на ребёнка: 50 / 75 / 100% регионального ПМ на ребёнка. Для Москвы 2026 — 10 951,50 / 16 427,25 / 21 903 ₽. Назначается при среднедушевом доходе ниже регионального ПМ на душу и с учётом имущественной оценки (income-правило ещё уточняется)."),
        new AmountRange(
            new MoneyAmount(10_951.50m, Currency.Rub, AmountPeriod.Monthly),
            new MoneyAmount(21_903m, Currency.Rub, AmountPeriod.Monthly)),
        new NoDeadline("Draft: the benefit depends on current household circumstances rather than a single ordinary filing deadline."),
        Moscow2026StageAvailability.AllChildStagesActive("Still relevant for older children while the program age limit continues to apply."),
        Moscow2026LegalSources.FederalPortal("Госуслуги/СФР: единое пособие на ребёнка", "gosuslugi-unified-child-benefit", "50/75/100% детского ПМ (Москва: 10 951,50 / 16 427,25 / 21 903 ₽); доход ниже 1 ПМ на душу + имущественный фильтр.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Gosuslugi or SFR", "Income and property information"),
        Moscow2026DraftContent.PortalApplicationSteps("Gosuslugi or SFR"),
        eligibilityRules: Moscow2026EligibilityRules.UnifiedChildBenefit());

    public static Benefit MatCapitalMonthlyPayment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MatCapitalMonthlyPayment,
        "mat-capital-monthly-payment",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier2Value,
        BenefitCategory.FamilyIncomeSupport,
        Moscow2026DraftContent.Copy(
            "Выплата из маткапитала",
            "Ежемесячная выплата из средств материнского капитала",
            "Ежемесячная выплата из уже предоставленного материнского капитала.",
            "Ежемесячная выплата из средств маткапитала на ребёнка до 3 лет. Размер равен детскому ПМ региона (для Москвы 2026 — 21 903 ₽/мес.). Назначается при среднедушевом доходе не более 2 ПМ на душу населения."),
        new FixedAmount(CreateLivingMinimumChildAmount()),
        new ActiveUntilChildAge(36, "The MVP treats the monthly payment from matcapital as relevant until the child reaches three years."),
        Moscow2026StageAvailability.UntilThreeYears("The ordinary monthly-payment route is treated as expired after the child reaches three years."),
        Moscow2026LegalSources.FederalLaw("Ежемесячная выплата из средств материнского капитала", "256-ФЗ", "Размер = детский ПМ региона (Москва 2026 — 21 903 ₽); порог дохода — 2 ПМ на душу.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("Gosuslugi or SFR", "Matcapital certificate or balance information"),
        Moscow2026DraftContent.PortalApplicationSteps("Gosuslugi or SFR"),
        eligibilityRules: Moscow2026EligibilityRules.MatCapitalMonthlyPayment());

    public static Benefit ChildCareLeaveRight { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.ChildCareLeaveRight,
        "child-care-leave-right",
        BenefitType.OngoingRight,
        BenefitTier.Tier1Core,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Отпуск по уходу за ребёнком",
            "Право на отпуск по уходу за ребёнком (ст. 256 ТК РФ)",
            "Отпуск может оформить не только мать: отец, бабушка, дед, другой родственник или опекун, фактически осуществляющий уход за ребёнком.",
            "Отпуск по уходу за ребёнком предоставляется до достижения им 3 лет и может использоваться полностью или по частям. Его вправе оформить мать, отец, бабушка, дед, другой родственник или опекун, фактически ухаживающий за ребёнком. На время отпуска сохраняется место работы (должность). Пособие по уходу до 1,5 лет — отдельная карточка."),
        new StatusOnlyAmount("Сохраняется место работы; пособие по уходу до 1,5 лет — отдельная карточка."),
        new ActiveUntilChildAge(36, "The MVP treats childcare leave as active until the child reaches three years."),
        Moscow2026StageAvailability.UntilThreeYears("The ordinary childcare-leave period is treated as expired after the child reaches three years."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: отпуск по уходу за ребёнком", "ст. 256", "Может использовать любой фактически ухаживающий родственник/опекун, не только родитель.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Child birth certificate and leave request"),
        Moscow2026DraftContent.EmployerRightSteps("childcare leave right"),
        eligibilityRules: Moscow2026EligibilityRules.ChildcareEmploymentRight());

    public static Benefit ParentDismissalProtection { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.ParentDismissalProtection,
        "parent-dismissal-protection",
        BenefitType.OngoingRight,
        BenefitTier.Tier1Core,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Защита родителя от увольнения",
            "Дополнительные гарантии при увольнении для отдельных категорий с детьми (ст. 261 ТК РФ)",
            "Гарантия распространяется не на всех родителей, а на отдельные категории.",
            "Запрет увольнения по инициативе работодателя действует для отдельных категорий: матери ребёнка до 3 лет; одинокой матери ребёнка до 16 лет (или ребёнка-инвалида до 18 лет); других лиц, воспитывающих таких детей без матери; единственного кормильца ребёнка-инвалида до 18 лет или ребёнка до 3 лет в многодетной семье. Исключения — ликвидация организации / прекращение ИП и отдельные виновные основания. Это трудовая гарантия, а не денежная выплата."),
        new StatusOnlyAmount("Трудовая гарантия при увольнении; прямой денежной суммы нет."),
        new ActiveUntilChildAge(36, "The MVP marks the core protection as relevant until the child reaches three years."),
        Moscow2026StageAvailability.UntilThreeYears("The child-age-based protection is treated as expired after the child reaches three years."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: гарантии при увольнении для лиц с семейными обязанностями", "ст. 261", "Узкие категории (одинокая мать, единственный кормилец и т.д.) с исключениями; не любой родитель.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Child birth certificate and employment documents"),
        Moscow2026DraftContent.EmployerRightSteps("parent dismissal protection"),
        eligibilityRules: Moscow2026EligibilityRules.ChildcareEmploymentRight());

    public static Benefit FeedingBreaksRight { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.FeedingBreaksRight,
        "feeding-breaks-right",
        BenefitType.OngoingRight,
        BenefitTier.Tier2Value,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Перерывы для кормления",
            "Дополнительные перерывы для кормления ребёнка (ст. 258 ТК РФ)",
            "Работающая женщина с детьми до 1,5 лет имеет право на дополнительные оплачиваемые перерывы для кормления (не любой работающий родитель).",
            "Перерывы предоставляются не реже чем каждые 3 часа, продолжительностью не менее 30 минут каждый; при двух и более детях до 1,5 лет — не менее 1 часа. Перерывы включаются в рабочее время и оплачиваются по среднему заработку. По заявлению женщины их можно присоединить к перерыву для отдыха и питания либо перенести на начало или конец рабочего дня."),
        new StatusOnlyAmount("Перерывы оплачиваются по среднему заработку; отдельной выплаты-суммы у этого права нет."),
        new ActiveUntilChildAge(18, "The MVP treats feeding breaks as active until the child reaches 1.5 years."),
        Moscow2026StageAvailability.UntilEighteenMonths("The ordinary feeding-break period is treated as expired after the child reaches 1.5 years."),
        Moscow2026LegalSources.LaborSource("Трудовой кодекс РФ: перерывы для кормления ребёнка", "ст. 258", "Право работающей женщины с детьми до 1,5 лет; перерывы оплачиваются по среднему заработку.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.EmploymentRightDocuments("Child-age document and work schedule information"),
        Moscow2026DraftContent.EmployerRightSteps("feeding-break right"),
        eligibilityRules: Moscow2026EligibilityRules.ChildcareEmploymentRight());

    public static Benefit ChildMilkKitchen { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.ChildMilkKitchen,
        "child-milk-kitchen",
        BenefitType.NaturalSupport,
        BenefitTier.Tier2Value,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Бесплатные продукты для малыша — молочная кухня",
            "Обеспечение ребёнка продуктами питания по линии молочной кухни",
            "Натуральная поддержка для ребёнка через московскую систему молочной кухни.",
            "Это натуральная поддержка (продукты) для ребёнка, а не денежная выплата. Маршрут — через детскую поликлинику и mos.ru. Возрастные нормы, ассортимент и порядок выдачи устанавливаются приказом ДЗМ и не финализируются здесь без выгрузки актуального приказа."),
        new NaturalSupport("Product support for the child through the Moscow milk-kitchen system."),
        new ActiveUntilChildAge(36, "The MVP treats child milk-kitchen support as relevant until the child reaches three years."),
        Moscow2026StageAvailability.UntilThreeYears("The ordinary child milk-kitchen route is treated as expired after the child reaches three years."),
        Moscow2026LegalSources.MoscowPortal("mos.ru: молочная кухня для ребёнка", "mosru-child-milk-kitchen", "Маршрут подтверждён; возрастные нормы и ассортимент — HOLD до выгрузки приказа ДЗМ.", verificationStatus: VerificationStatus.NeedsReview),
        Moscow2026DraftContent.ApplicationDocuments("Clinic, mos.ru, or distribution point", "Medical prescription or child clinic referral"),
        Moscow2026DraftContent.PortalApplicationSteps("clinic, mos.ru, or the listed distribution point"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());
}
