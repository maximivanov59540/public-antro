namespace Antro.Application;

public interface IAntroClock
{
    DateOnly Today { get; }
}

public sealed class SystemAntroClock : IAntroClock
{
    // Demo strategy A: Maria stays 14 days old relative to the current app date,
    // so the scenario remains stable without hardcoded dashboard results.
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
