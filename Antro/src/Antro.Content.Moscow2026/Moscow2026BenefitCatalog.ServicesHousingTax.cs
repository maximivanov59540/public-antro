using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static partial class Moscow2026BenefitCatalog
{
    public static Benefit KindergartenEnrollment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.KindergartenEnrollment,
        "kindergarten-enrollment",
        BenefitType.AdministrativeAction,
        BenefitTier.Tier1Core,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Запись в детский сад",
            "Постановка ребёнка в очередь в детский сад",
            "Административный маршрут, который лучше не откладывать перед детским садом.",
            "Draft MVP summary: this is the queue-and-enrollment route for kindergarten rather than a money payment. Final timing guidance still needs review."),
        new StatusOnlyAmount("Administrative route; no direct monetary amount."),
        new NoDeadline("Draft: admission waves and queue timing matter, but the MVP schema does not yet model a single formal filing deadline."),
        Moscow2026StageAvailability.PreschoolAndOlder(),
        Moscow2026LegalSources.MoscowPortal("mos.ru: запись в детский сад", "mosru-kindergarten-enrollment", "Draft source marker for the Moscow kindergarten route."),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru", "Child registration and address information"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());

    public static Benefit KindergartenFeeCompensation { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.KindergartenFeeCompensation,
        "kindergarten-fee-compensation",
        BenefitType.IncomeDependentPayment,
        BenefitTier.Tier2Value,
        BenefitCategory.Childcare,
        Moscow2026DraftContent.Copy(
            "Компенсация платы за детский сад",
            "Компенсация части родительской платы за детский сад",
            "Часть родительской платы за детский сад может компенсироваться. Размер зависит от очерёдности ребёнка и региональных правил — не единые 70% для всех.",
            "Федеральный минимум компенсации родительской платы: не менее 20% на первого ребёнка, 50% — на второго, 70% — на третьего и последующих детей. Субъект РФ вправе устанавливать критерии нуждаемости и порядок выплаты, поэтому для Москвы конкретный размер и маршрут уточняются отдельно."),
        new PercentageSubsidy(20m, null, "Федеральный минимум: не менее 20 / 50 / 70% по очерёдности ребёнка (первый / второй / третий и последующие). Регион вправе уточнять размер и критерии нуждаемости — нельзя применять единые 70% ко всем."),
        new NoDeadline("Draft: ongoing preschool attendance matters more than a single ordinary filing deadline."),
        Moscow2026StageAvailability.PreschoolAndOlder(),
        Moscow2026LegalSources.FederalLaw("Закон «Об образовании в РФ»: компенсация родительской платы", "273-ФЗ", "Минимум 20 / 50 / 70% по очерёдности (ст. 65); порядок и критерии — региональные.", articleOrClause: "ст. 65", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru, Gosuslugi, or the kindergarten route", "Kindergarten attendance and payment proof"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru, Gosuslugi, or the kindergarten route"),
        eligibilityRules: Moscow2026EligibilityRules.KindergartenFeeCompensation());

    public static Benefit SchoolMeals { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.SchoolMeals,
        "school-meals",
        BenefitType.NaturalSupport,
        BenefitTier.Tier2Value,
        BenefitCategory.DocumentsAndServices,
        Moscow2026DraftContent.Copy(
            "Бесплатное питание в школе",
            "Бесплатное питание для школьника",
            "Бесплатный завтрак для учеников 1–4 классов и льготное питание для отдельных категорий.",
            "Бесплатный завтрак для учеников 1–4 классов предоставляется без отдельного заявления. Льготное питание положено отдельным категориям: дети из многодетных и малообеспеченных семей, дети-сироты, дети-инвалиды и другие — точный перечень берётся со страницы услуги mos.ru и из школьного регламента. Срок предоставления услуги — до 8 рабочих дней. Право на питание не вытекает из карты москвича автоматически."),
        new NaturalSupport("Бесплатный завтрак 1–4 классов и льготное питание для отдельных категорий школьников."),
        new NoDeadline("Draft: relevance usually depends on school attendance and category confirmation rather than a one-time deadline."),
        Moscow2026StageAvailability.SchoolAge(),
        Moscow2026LegalSources.MoscowPortal("mos.ru: бесплатное и льготное питание обучающихся", "mosru-school-meals", "Завтрак 1–4 без заявления; льготные категории; срок 8 рабочих дней; не основано на карте москвича.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("school, mos.ru, or the education route", "School enrollment and category confirmation"),
        Moscow2026DraftContent.PortalApplicationSteps("school, mos.ru, or the education route"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());

    public static Benefit MoscowStudentCard { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MoscowStudentCard,
        "moscow-student-card",
        BenefitType.NaturalSupport,
        BenefitTier.Tier2Value,
        BenefitCategory.DocumentsAndServices,
        Moscow2026DraftContent.Copy(
            "Карта москвича школьника",
            "Социальная карта школьника",
            "Городская сервисная карта школьника: проход и питание в школе, библиотека, скидки, транспортные сценарии, «Музеи — детям». Это сервисный инструмент, а не отдельная льгота.",
            "Карта москвича школьника даёт доступ к городским сервисам: проход и оплата питания в школе, библиотека, скидки, транспортные сценарии, проект «Музеи — детям». Важно: право на бесплатное питание или бесплатный проезд не возникает из самой карты автоматически — оно подтверждается отдельными основаниями. Заявление подаётся через mos.ru, карта обычно готовится до 30 дней."),
        new NaturalSupport("Сервисная карта школьника (сервисы и доступ, не денежная выплата и не отдельная льгота)."),
        new NoDeadline("Draft: relevance starts with school status and the route is usually administrative rather than deadline-driven."),
        Moscow2026StageAvailability.SchoolAge(),
        Moscow2026LegalSources.MoscowPortal("mos.ru: карта москвича для школьника", "mosru-student-card", "Сервисная карта; питание/проезд из неё автоматически не вытекают; готовится до 30 дней.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("mos.ru", "School enrollment proof and photo or identity data"),
        Moscow2026DraftContent.PortalApplicationSteps("mos.ru"),
        eligibilityRules: Moscow2026EligibilityRules.MoscowRegistrationOnly());

    public static Benefit FamilyMortgage { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.FamilyMortgage,
        "family-mortgage",
        BenefitType.HousingSupport,
        BenefitTier.Tier1Core,
        BenefitCategory.Housing,
        Moscow2026DraftContent.Copy(
            "Семейная ипотека",
            "Льготная программа «Семейная ипотека»",
            "Льготная ипотека по ставке до 6% для семей с детьми. Первоначальный взнос — от 20%.",
            "Льготная программа «Семейная ипотека»: ставка до 6%, первоначальный взнос от 20%. Лимит льготного кредита — 12 млн ₽ для Москвы, Московской области, Санкт-Петербурга и Ленинградской области; 6 млн ₽ для остальных регионов. Право имеют семьи с ребёнком до 6 лет включительно; с двумя и более несовершеннолетними детьми; с несовершеннолетним ребёнком-инвалидом. С 01.02.2026 для супругов — один кредитный договор с обоими супругами как созаёмщиками (кроме исключений). Кредитный договор можно заключить до 31.12.2030."),
        new PercentageSubsidy(6m, null, "Ставка до 6%; взнос от 20%; лимит кредита 12 млн ₽ (Москва/МО/СПб/ЛО) или 6 млн ₽ (остальные регионы); договор до 31.12.2030."),
        new NoDeadline("Программа действует до 31.12.2030 (срок заключения кредитного договора); это срок программы, а не персональный дедлайн от рождения."),
        Moscow2026StageAvailability.HousingLongTail("Can matter during pregnancy when the family is planning housing under current federal rules.", "May remain relevant later if the family is still choosing a housing route."),
        Moscow2026LegalSources.GovernmentResolution("ДОМ.РФ: Семейная ипотека", "family-mortgage-program", "6% / взнос 20% / лимит 12 млн (Москва/МО/СПб/ЛО) или 6 млн; договор до 31.12.2030; с 01.02.2026 супруги-созаёмщики.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("bank, DOM.RF, or official program route", "Mortgage and housing transaction documents"),
        Moscow2026DraftContent.PortalApplicationSteps("bank, DOM.RF, or the official program route"),
        eligibilityRules: Moscow2026EligibilityRules.FamilyMortgage(),
        priorityHint: Moscow2026PriorityHints.HousingRecommendation);

    public static Benefit MatCapitalForHousing { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MatCapitalForHousing,
        "mat-capital-for-housing",
        BenefitType.HousingSupport,
        BenefitTier.Tier1Core,
        BenefitCategory.Housing,
        Moscow2026DraftContent.Copy(
            "Маткапитал на жильё",
            "Использование материнского капитала на улучшение жилищных условий",
            "Средства маткапитала можно направить на жильё, ипотеку или строительство в разрешённых случаях.",
            "Это не новая выплата, а направление использования уже имеющегося материнского капитала. Доступная сумма равна остатку сертификата (актуальные размеры — см. карточку «Материнский капитал»), а не отдельному фиксированному размеру."),
        new StatusOnlyAmount("Сумма равна остатку сертификата материнского капитала; отдельного фиксированного размера у этой меры нет."),
        new NoDeadline("Matcapital use for housing is treated as an ongoing route without an ordinary separate filing deadline."),
        Moscow2026StageAvailability.PostBirthAllStages("Still relevant later because housing use of matcapital can happen beyond the newborn period."),
        Moscow2026LegalSources.FederalLaw("Использование маткапитала на жильё", "256-ФЗ", "Использование остатка сертификата на улучшение жилищных условий; не хранить фиксированную сумму.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("SFR, bank, or housing route", "Housing transaction or mortgage documents"),
        Moscow2026DraftContent.PortalApplicationSteps("SFR, bank, or the housing route"),
        eligibilityRules: Moscow2026EligibilityRules.MatCapitalForHousing());

    public static Benefit LargeFamilyMortgagePayoff { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.LargeFamilyMortgagePayoff,
        "large-family-mortgage-payoff",
        BenefitType.HousingSupport,
        BenefitTier.Tier1Core,
        BenefitCategory.Housing,
        Moscow2026DraftContent.Copy(
            "450 000 ₽ многодетным на ипотеку",
            "Государственная поддержка многодетных семей в погашении ипотеки",
            "До 450 000 ₽ безналично на погашение ипотечного кредита для семей, где с 2019 года родился третий или последующий ребёнок.",
            "Выплата до 450 000 ₽ направляется безналично на погашение ипотечного кредита. Если остаток долга меньше 450 000 ₽, выплата равна остатку. Право имеют семьи, в которых с 01.01.2019 по 31.12.2030 родился третий или последующий ребёнок (усыновлённые дети учитываются). Заявитель должен быть заёмщиком или созаёмщиком; заявитель и дети — граждане РФ. Кредитный договор должен быть заключён до 01.07.2031. Подача — через банк или Госуслуги. Это отдельная мера, не путать с выплатой на первого ребёнка."),
        new UpToAmount(new MoneyAmount(450_000m, Currency.Rub, AmountPeriod.OneTime)),
        new NoDeadline("Кредитный договор должен быть заключён до 01.07.2031; это срок программы, а не персональный дедлайн от рождения ребёнка."),
        Moscow2026StageAvailability.HousingLongTail(
            "Может быть актуально уже при планировании, если в семье ожидается третий или последующий ребёнок.",
            "Остаётся актуальной, пока действует ипотека и сохраняется право многодетной семьи."),
        Moscow2026LegalSources.FederalLaw("Господдержка многодетных семей в погашении ипотеки (450 000 ₽)", "157-ФЗ", "До 450 000 ₽ на погашение ипотеки; третий+ ребёнок 2019–2030; договор до 01.07.2031 (ДОМ.РФ, Минфин).", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.ApplicationDocuments("банк или Госуслуги", "Кредитный договор и документы на детей"),
        Moscow2026DraftContent.PortalApplicationSteps("банк или Госуслуги"),
        eligibilityRules: Moscow2026EligibilityRules.LargeFamilyMortgagePayoff(),
        priorityHint: Moscow2026PriorityHints.HousingRecommendation);

    public static Benefit FederalYoungFamilyHousingProgram { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.FederalYoungFamilyHousingProgram,
        "federal-young-family-housing-program",
        BenefitType.HousingSupport,
        BenefitTier.Tier3HiddenGem,
        BenefitCategory.Housing,
        Moscow2026DraftContent.Copy(
            "Программа «Молодая семья»",
            "Федеральная программа обеспечения жильём молодых семей",
            "Субсидийный жилищный маршрут для молодой семьи при соблюдении условий программы.",
            "Draft MVP summary: this is a housing queue and subsidy program, not a universal automatic payment. Final queue rules and local practice still need review."),
        new PercentageSubsidy(35m, null, "Draft scaffold for the typical subsidy share. Final legal parameters still need review."),
        new NoDeadline("Draft: queue entry, region practice, and program validity matter more than a single ordinary filing deadline."),
        Moscow2026StageAvailability.HousingLongTail("Can be relevant already during pregnancy while the family plans long-term housing options.", "Still relevant later if the family remains eligible and continues through the queue."),
        Moscow2026LegalSources.FederalPortal("Госуслуги: программа «Молодая семья»", "gosuslugi-young-family-housing-program", "Draft official-source marker for the federal young-family housing route."),
        Moscow2026DraftContent.ApplicationDocuments("municipal housing route or Gosuslugi", "Queue and housing-need documents"),
        Moscow2026DraftContent.PortalApplicationSteps("municipal housing route or Gosuslugi"),
        eligibilityRules: Moscow2026EligibilityRules.FederalYoungFamilyHousingProgram(),
        priorityHint: Moscow2026PriorityHints.HousingRecommendation);

    public static Benefit ChildTaxDeduction { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.ChildTaxDeduction,
        "child-tax-deduction",
        BenefitType.TaxBenefit,
        BenefitTier.Tier2Value,
        BenefitCategory.Taxes,
        Moscow2026DraftContent.Copy(
            "Налоговый вычет на ребёнка",
            "Стандартный налоговый вычет на ребёнка",
            "Работающий родитель может уменьшать налоговую базу по НДФЛ на ребёнка. Это налоговая экономия (обычно 13% от базы вычета), а не отдельная выплата.",
            "Стандартный налоговый вычет на ребёнка. База вычета в месяц: 1-й ребёнок — 1 400 ₽, 2-й — 2 800 ₽, 3-й и последующие — 6 000 ₽; на ребёнка-инвалида — дополнительно 12 000 ₽ для родителей/усыновителей. Применяется до месяца, в котором доход с начала года превысил 450 000 ₽. В UI показывается налоговая экономия (≈13% от базы), а не «выплата»."),
        new ChoiceAmount(
        [
            new ChoiceAmountOption(new MoneyAmount(1_400m, Currency.Rub, AmountPeriod.Monthly), label: "База вычета на первого ребёнка", childOrder: ChildOrder.First),
            new ChoiceAmountOption(new MoneyAmount(2_800m, Currency.Rub, AmountPeriod.Monthly), label: "База вычета на второго ребёнка", childOrder: ChildOrder.Second),
            new ChoiceAmountOption(new MoneyAmount(6_000m, Currency.Rub, AmountPeriod.Monthly), label: "База вычета на третьего и последующего ребёнка", childOrder: ChildOrder.ThirdOrLater)
        ],
        "Месячная база вычета по НК РФ ст. 218. На ребёнка-инвалида — дополнительный вычет 12 000 ₽. Это база налоговой экономии (≈13%), а не сумма к выплате; вычет применяется до достижения дохода 450 000 ₽ с начала года."),
        new NoDeadline("Draft: the deduction is usually handled through employer payroll or tax filing rather than a one-time benefit deadline."),
        Moscow2026StageAvailability.AllChildStagesActive("Still relevant for older children while the tax deduction remains available."),
        Moscow2026LegalSources.TaxSource("Стандартный налоговый вычет на ребёнка", "ст. 218", "Базы 1 400 / 2 800 / 6 000 ₽ и доход-кат-офф 450 000 ₽ (ФНС). Заменяет прежние 1 400 / 1 400 / 3 000 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.TaxDocuments("Employer payroll statement or deduction request"),
        Moscow2026DraftContent.TaxBenefitSteps(),
        eligibilityRules: Moscow2026EligibilityRules.TaxBenefitForWorkingParent());

    public static Benefit EducationTaxDeduction { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.EducationTaxDeduction,
        "education-tax-deduction",
        BenefitType.TaxBenefit,
        BenefitTier.Tier2Value,
        BenefitCategory.Taxes,
        Moscow2026DraftContent.Copy(
            "Вычет на обучение ребёнка",
            "Социальный налоговый вычет на обучение ребёнка",
            "Можно вернуть часть налога при оплате обучения ребёнка в допустимых пределах. Это возврат НДФЛ (обычно до 13% лимита), а не отдельная выплата.",
            "Социальный налоговый вычет на обучение ребёнка. Лимит расходов — 110 000 ₽ на каждого ребёнка (с 01.01.2024), в общей сумме на обоих родителей/опекунов. Возврат обычно до 13% лимита при наличии НДФЛ; декларационный возврат — в пределах 3 лет."),
        new UpToAmount(new MoneyAmount(110_000m, Currency.Rub, AmountPeriod.Yearly)),
        new NoDeadline("Draft: this deduction is usually claimed through tax filing periods rather than a catalog-specific benefit deadline."),
        Moscow2026StageAvailability.SchoolAge(),
        Moscow2026LegalSources.TaxSource("Социальный налоговый вычет на обучение ребёнка", "ст. 219", "Лимит 110 000 ₽ на ребёнка с 01.01.2024 (ФНС). Заменяет прежние 50 000 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.TaxDocuments("Education contract and payment receipts"),
        Moscow2026DraftContent.TaxBenefitSteps(),
        eligibilityRules: Moscow2026EligibilityRules.TaxBenefitForWorkingParent());

    public static Benefit MedicalTaxDeduction { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.MedicalTaxDeduction,
        "medical-tax-deduction",
        BenefitType.TaxBenefit,
        BenefitTier.Tier2Value,
        BenefitCategory.Taxes,
        Moscow2026DraftContent.Copy(
            "Вычет на лечение ребёнка",
            "Социальный налоговый вычет на лечение ребёнка",
            "Можно вернуть часть налога по расходам на лечение ребёнка в допустимых пределах. Это возврат НДФЛ (обычно до 13%), а не отдельная выплата.",
            "Социальный налоговый вычет на лечение ребёнка. Общий лимит социальных расходов — 150 000 ₽/год; дорогостоящее лечение принимается по фактическим расходам вне этого лимита. Возврат обычно 13% при наличии НДФЛ; декларационный возврат — в пределах 3 лет."),
        new UpToAmount(new MoneyAmount(150_000m, Currency.Rub, AmountPeriod.Yearly)),
        new NoDeadline("Draft: this deduction is usually claimed through tax filing periods rather than a catalog-specific benefit deadline."),
        Moscow2026StageAvailability.AllChildStagesActive("Still relevant later while eligible child medical expenses continue."),
        Moscow2026LegalSources.TaxSource("Социальный налоговый вычет на лечение ребёнка", "ст. 219", "Общий лимит 150 000 ₽/год; дорогостоящее лечение — отдельно (ФНС). Заменяет прежние 120 000 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.TaxDocuments("Medical contract and payment receipts"),
        Moscow2026DraftContent.TaxBenefitSteps(),
        eligibilityRules: Moscow2026EligibilityRules.TaxBenefitForWorkingParent());

    public static Benefit EmployerMaterialAidTaxBenefit { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.EmployerMaterialAidTaxBenefit,
        "employer-material-aid-tax-benefit",
        BenefitType.TaxBenefit,
        BenefitTier.Tier3HiddenGem,
        BenefitCategory.Taxes,
        Moscow2026DraftContent.Copy(
            "Налоговая льгота на матпомощь от работодателя",
            "Налоговая льгота на материальную помощь работодателя при рождении ребёнка",
            "Материальная помощь от работодателя после рождения ребёнка освобождается от НДФЛ в пределах лимита. Это налоговое освобождение, а не бюджетная выплата — не «получите 1 млн ₽».",
            "Единовременная материальная помощь работнику-родителю при рождении/усыновлении/опеке освобождается от НДФЛ до 1 000 000 ₽ на каждого ребёнка. Выгода — это сэкономленный НДФЛ с суммы помощи, а не сама сумма. Выплата должна быть произведена в течение первого года после рождения."),
        new UpToAmount(new MoneyAmount(1_000_000m, Currency.Rub, AmountPeriod.OneTime)),
        new NoDeadline("Draft: the tax route depends on employer payment timing rather than a separate catalog filing deadline."),
        Moscow2026StageAvailability.EmployerBirthYearTaxBenefit(),
        Moscow2026LegalSources.TaxSource("Освобождение от НДФЛ матпомощи работодателя при рождении ребёнка", "ст. 217 п. 8", "Лимит освобождения 1 000 000 ₽ на ребёнка (ФНС). Это налоговое освобождение, не выплата. Заменяет прежние 50 000 ₽.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.TaxDocuments("Employer payment order or statement"),
        Moscow2026DraftContent.TaxBenefitSteps(),
        eligibilityRules: Moscow2026EligibilityRules.TaxBenefitForWorkingParent(),
        priorityHint: Moscow2026PriorityHints.HiddenTaxRoute);

    public static Benefit AnnualFamilyTaxPayment { get; } = Moscow2026BenefitFactory.Create(
        Moscow2026BenefitIds.AnnualFamilyTaxPayment,
        "annual-family-tax-payment",
        BenefitType.TaxBenefit,
        BenefitTier.Tier3HiddenGem,
        BenefitCategory.Taxes,
        Moscow2026DraftContent.Copy(
            "Ежегодная семейная выплата",
            "Ежегодная семейная налоговая выплата",
            "Ежегодная выплата работающим родителям с двумя и более детьми: возврат части уплаченного НДФЛ.",
            "Ежегодная семейная выплата для работающих родителей с двумя и более детьми. Сумма не фиксированная: расчётный НДФЛ за предыдущий год минус налог с того же дохода по ставке 6%. Условия: дети до 18 лет (или 18–23 при очном обучении); получатель — гражданин РФ, налоговый резидент с доходом под НДФЛ 13%; среднедушевой доход ≤ 1,5 ПМ трудоспособного населения региона; нет задолженности по алиментам; имущественный фильтр. Заявление подаётся ежегодно с 1 июня по 1 октября года, следующего за годом исчисления НДФЛ (в 2026 году — за доходы 2025 года)."),
        new StatusOnlyAmount("Сумма не фиксирована: расчётный НДФЛ за прошлый год минус налог по ставке 6% с того же дохода."),
        new NoDeadline("Заявление подаётся ежегодно с 1 июня по 1 октября — это повторяющееся окно подачи, а не единый дедлайн от рождения."),
        Moscow2026StageAvailability.AllChildStagesActive("Still relevant later while the family remains in scope of the annual tax-payment program."),
        Moscow2026LegalSources.FederalLaw("Ежегодная семейная выплата (возврат части НДФЛ)", "179-ФЗ", "ФЗ от 13.07.2024 № 179-ФЗ; формула НДФЛ − 6%; окно подачи 1 июня–1 октября; доход ≤ 1,5 ПМ трудоспособного.", lastCheckedAt: Moscow2026LegalSources.RegisterCheckDate, verificationStatus: VerificationStatus.Checked),
        Moscow2026DraftContent.TaxDocuments("Tax filing and family-status evidence"),
        Moscow2026DraftContent.TaxBenefitSteps(),
        eligibilityRules: Moscow2026EligibilityRules.AnnualFamilyTaxPayment(),
        priorityHint: Moscow2026PriorityHints.HiddenTaxRoute);
}
