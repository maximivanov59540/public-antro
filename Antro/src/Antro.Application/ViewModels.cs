using Antro.Domain;

namespace Antro.Application;

public sealed record StageShowcaseViewModel(
    LifeStage Stage,
    string Title,
    string Subtitle,
    string SummaryText,
    IReadOnlyList<BenefitSectionViewModel> Sections,
    StageShowcaseHeaderInfoViewModel HeaderInfo);

public sealed record BenefitSectionViewModel(
    string SectionKey,
    string SectionHeaderText,
    string SectionColorClass,
    string SectionSubtitleText,
    IReadOnlyList<BenefitCardViewModel> Cards);

public sealed record StageShowcaseHeaderInfoViewModel(
    int UrgentCount,
    int RightsCount,
    string RegionName,
    string TotalAmountText,
    IReadOnlyList<AmountCategoryViewModel> AmountsByCategory);

public sealed record AmountCategoryViewModel(
    string Label,
    string AmountText);

public sealed record BenefitCardViewModel(
    BenefitId BenefitId,
    string Slug,
    string Href,
    string Title,
    string Subtitle,
    string CategoryKey,
    StatusPillViewModel Availability,
    AmountDisplayViewModel? Amount,
    DeadlineBadgeViewModel Deadline,
    string SummaryText,
    int PriorityScore,
    IReadOnlyList<StatusPillViewModel> Badges,
    bool IsMuted,
    string IconKey,
    string SectionCategoryKey,
    BenefitCategory BaseCategory);

public sealed record AmountDisplayViewModel(
    string Text,
    string KindKey,
    bool IsMonetary,
    bool CanParticipateInTotals,
    string? DetailsText);

public sealed record DeadlineBadgeViewModel(
    string Text,
    string SemanticKey,
    string KindKey,
    DateOnly? DeadlineDate,
    int? DaysLeft,
    string Explanation);

public sealed record StatusPillViewModel(
    string Text,
    string SemanticKey);

public sealed record DashboardViewModel(
    string Title,
    DashboardStageViewModel Stage,
    DashboardSummaryViewModel Summary,
    IReadOnlyList<DeadlinePreviewViewModel> UrgentDeadlines,
    IReadOnlyList<RightPreviewViewModel> CurrentRights,
    IReadOnlyList<BenefitSectionViewModel> AllBenefitSections);

public sealed record DashboardStageViewModel(
    string Eyebrow,
    string Label,
    string Description);

public sealed record DashboardSummaryViewModel(
    string SummaryText,
    IReadOnlyList<DashboardSummaryBucketViewModel> Buckets,
    int TotalBenefitsCount,
    int AvailableNowCount,
    int NeedsMoreInfoCount,
    int UrgentCount);

public sealed record DeadlinePreviewViewModel(
    BenefitId BenefitId,
    string Href,
    string Title,
    StatusPillViewModel Availability,
    DeadlineBadgeViewModel Deadline);

public sealed record RightPreviewViewModel(
    BenefitId BenefitId,
    string Href,
    string Title,
    string Subtitle,
    StatusPillViewModel Availability);

public sealed record DashboardSummaryBucketViewModel(
    string Title,
    string ValueText,
    string Description,
    string SemanticKey,
    bool IsMuted);

public sealed record BenefitDetailViewModel(
    BenefitId BenefitId,
    string Slug,
    string Title,
    string Subtitle,
    StatusPillViewModel Availability,
    AmountDisplayViewModel? Amount,
    DeadlineBadgeViewModel Deadline,
    string WhatIsIt,
    IReadOnlyList<ConditionViewModel> Conditions,
    IReadOnlyList<DocumentRequirementViewModel> Documents,
    IReadOnlyList<ActionStepViewModel> ActionSteps,
    IReadOnlyList<LegalSourceViewModel> LegalSources);

public sealed record ConditionViewModel(
    string Text,
    StatusPillViewModel Status,
    string? Explanation);

public sealed record DocumentRequirementViewModel(
    string Title,
    string? Description,
    string? Notes);

public sealed record ActionStepViewModel(
    int Order,
    string Title,
    string? Description,
    string? Notes);

public sealed record LegalSourceViewModel(
    string Title,
    string SourceIdentifier,
    string? ArticleOrClause,
    string LevelLabel,
    string VerificationLabel,
    DateOnly? LastCheckedAt,
    string? Notes);
