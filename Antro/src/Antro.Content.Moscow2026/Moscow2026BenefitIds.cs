using Antro.Domain;

namespace Antro.Content.Moscow2026;

public static class Moscow2026BenefitIds
{
    public static BenefitId PregnancyMonthlyBenefit { get; } = new("moscow2026.pregnancy-monthly-benefit");
    public static BenefitId MaternityLeaveBenefit { get; } = new("moscow2026.maternity-leave-benefit");
    public static BenefitId MoscowEarlyPregnancyRegistrationPayment { get; } = new("moscow2026.moscow-early-pregnancy-registration-payment");
    public static BenefitId PregnancyMilkKitchen { get; } = new("moscow2026.pregnancy-milk-kitchen");
    public static BenefitId PregnancyDismissalProtection { get; } = new("moscow2026.pregnancy-dismissal-protection");
    public static BenefitId PregnancyLightWorkRight { get; } = new("moscow2026.pregnancy-light-work-right");
    public static BenefitId PregnancyPartTimeRight { get; } = new("moscow2026.pregnancy-part-time-right");
    public static BenefitId MoscowNewbornGiftSet { get; } = new("moscow2026.moscow-newborn-gift-set");
    public static BenefitId FederalBirthGrant { get; } = new("moscow2026.federal-birth-grant");
    public static BenefitId MoscowBirthPayment { get; } = new("moscow2026.moscow-birth-payment");
    public static BenefitId MoscowYoungFamilyPayment { get; } = new("moscow2026.moscow-young-family-payment");
    public static BenefitId MaternityCapital { get; } = new("moscow2026.maternity-capital");
    public static BenefitId ChildCareMonthlyAllowance { get; } = new("moscow2026.child-care-monthly-allowance");
    public static BenefitId UnifiedChildBenefit { get; } = new("moscow2026.unified-child-benefit");
    public static BenefitId MatCapitalMonthlyPayment { get; } = new("moscow2026.mat-capital-monthly-payment");
    public static BenefitId ChildCareLeaveRight { get; } = new("moscow2026.child-care-leave-right");
    public static BenefitId ParentDismissalProtection { get; } = new("moscow2026.parent-dismissal-protection");
    public static BenefitId FeedingBreaksRight { get; } = new("moscow2026.feeding-breaks-right");
    public static BenefitId ChildMilkKitchen { get; } = new("moscow2026.child-milk-kitchen");
    public static BenefitId KindergartenEnrollment { get; } = new("moscow2026.kindergarten-enrollment");
    public static BenefitId KindergartenFeeCompensation { get; } = new("moscow2026.kindergarten-fee-compensation");
    public static BenefitId SchoolMeals { get; } = new("moscow2026.school-meals");
    public static BenefitId MoscowStudentCard { get; } = new("moscow2026.moscow-student-card");
    public static BenefitId FamilyMortgage { get; } = new("moscow2026.family-mortgage");
    public static BenefitId MatCapitalForHousing { get; } = new("moscow2026.mat-capital-for-housing");
    public static BenefitId LargeFamilyMortgagePayoff { get; } = new("moscow2026.large-family-mortgage-payoff");
    public static BenefitId FederalYoungFamilyHousingProgram { get; } = new("moscow2026.federal-young-family-housing-program");
    public static BenefitId ChildTaxDeduction { get; } = new("moscow2026.child-tax-deduction");
    public static BenefitId EducationTaxDeduction { get; } = new("moscow2026.education-tax-deduction");
    public static BenefitId MedicalTaxDeduction { get; } = new("moscow2026.medical-tax-deduction");
    public static BenefitId EmployerMaterialAidTaxBenefit { get; } = new("moscow2026.employer-material-aid-tax-benefit");
    public static BenefitId AnnualFamilyTaxPayment { get; } = new("moscow2026.annual-family-tax-payment");

    public static IReadOnlyList<BenefitId> All { get; } =
    [
        PregnancyMonthlyBenefit,
        MaternityLeaveBenefit,
        MoscowEarlyPregnancyRegistrationPayment,
        PregnancyMilkKitchen,
        PregnancyDismissalProtection,
        PregnancyLightWorkRight,
        PregnancyPartTimeRight,
        MoscowNewbornGiftSet,
        FederalBirthGrant,
        MoscowBirthPayment,
        MoscowYoungFamilyPayment,
        MaternityCapital,
        ChildCareMonthlyAllowance,
        UnifiedChildBenefit,
        MatCapitalMonthlyPayment,
        ChildCareLeaveRight,
        ParentDismissalProtection,
        FeedingBreaksRight,
        ChildMilkKitchen,
        KindergartenEnrollment,
        KindergartenFeeCompensation,
        SchoolMeals,
        MoscowStudentCard,
        FamilyMortgage,
        MatCapitalForHousing,
        LargeFamilyMortgagePayoff,
        FederalYoungFamilyHousingProgram,
        ChildTaxDeduction,
        EducationTaxDeduction,
        MedicalTaxDeduction,
        EmployerMaterialAidTaxBenefit,
        AnnualFamilyTaxPayment
    ];
}
