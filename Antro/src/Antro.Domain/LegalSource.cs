namespace Antro.Domain;

public enum LegalSourceLevel
{
    FederalLaw = 0,
    MoscowLaw = 1,
    GovernmentResolution = 2,
    OfficialPortal = 3,
    Other = 4
}

public enum VerificationStatus
{
    Draft = 0,
    Checked = 1,
    NeedsReview = 2,
    ConflictingSources = 3
}

public sealed record LegalSource
{
    public LegalSource(
        string title,
        string actNumberOrIdentifier,
        string? articleOrClause,
        LegalSourceLevel level,
        DateOnly? lastCheckedAt,
        VerificationStatus verificationStatus,
        string? notes)
    {
        Title = DomainGuard.RequiredText(title, nameof(title));
        ActNumberOrIdentifier = DomainGuard.RequiredText(actNumberOrIdentifier, nameof(actNumberOrIdentifier));
        ArticleOrClause = string.IsNullOrWhiteSpace(articleOrClause) ? null : articleOrClause.Trim();
        Level = level;
        LastCheckedAt = lastCheckedAt;
        VerificationStatus = verificationStatus;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public string Title { get; }

    public string ActNumberOrIdentifier { get; }

    public string? ArticleOrClause { get; }

    public LegalSourceLevel Level { get; }

    public DateOnly? LastCheckedAt { get; }

    public VerificationStatus VerificationStatus { get; }

    public string? Notes { get; }
}
