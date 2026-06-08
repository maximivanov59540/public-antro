namespace Antro.Domain;

public enum BenefitType
{
    DeadlineEvent = 0,
    OngoingRight = 1,
    Automatic = 2,
    IncomeDependentPayment = 3,
    TaxBenefit = 4,
    HousingSupport = 5,
    NaturalSupport = 6,
    AdministrativeAction = 7,
    InformationalRight = 8
}

public enum BenefitTier
{
    Tier1Core = 0,
    Tier2Value = 1,
    Tier3HiddenGem = 2
}

public enum BenefitCategory
{
    PregnancyAndBirth = 0,
    Childcare = 1,
    FamilyIncomeSupport = 2,
    Housing = 3,
    Taxes = 4,
    DocumentsAndServices = 5
}
