using Antro.Domain;

namespace Antro.Content.Moscow2026;

internal static class Moscow2026LegalSources
{
    /// <summary>
    /// Date the clean legal register (Antro_clean_legal_register_2026-06-05) was checked against
    /// official sources. Used as <see cref="LegalSource.LastCheckedAt"/> for finalized cards.
    /// </summary>
    public static readonly DateOnly RegisterCheckDate = new(2026, 6, 5);

    public static IReadOnlyList<LegalSource> FederalPortal(
        string title,
        string identifier,
        string notes,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            identifier,
            null,
            LegalSourceLevel.OfficialPortal,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> MoscowPortal(
        string title,
        string identifier,
        string notes,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            identifier,
            null,
            LegalSourceLevel.OfficialPortal,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> LaborSource(
        string title,
        string articleOrClause,
        string notes,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            "ТК РФ",
            articleOrClause,
            LegalSourceLevel.FederalLaw,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> FederalLaw(
        string title,
        string actNumber,
        string notes,
        string? articleOrClause = null,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            actNumber,
            articleOrClause,
            LegalSourceLevel.FederalLaw,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> MoscowLaw(
        string title,
        string actNumber,
        string notes,
        string? articleOrClause = null,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            actNumber,
            articleOrClause,
            LegalSourceLevel.MoscowLaw,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> GovernmentResolution(
        string title,
        string identifier,
        string notes,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            identifier,
            null,
            LegalSourceLevel.GovernmentResolution,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];

    public static IReadOnlyList<LegalSource> TaxSource(
        string title,
        string articleOrClause,
        string notes,
        DateOnly? lastCheckedAt = null,
        VerificationStatus verificationStatus = VerificationStatus.NeedsReview) =>
    [
        new LegalSource(
            title,
            "НК РФ",
            articleOrClause,
            LegalSourceLevel.FederalLaw,
            lastCheckedAt,
            verificationStatus,
            notes)
    ];
}
