using Antro.Domain;

namespace Antro.Web;

public sealed class InMemoryUserProfileState
{
    public const string DefaultDashboardRoute = "/dashboard";

    public UserProfile? CurrentProfile { get; private set; }

    public string CurrentDashboardRoute { get; private set; } = DefaultDashboardRoute;

    public void Set(UserProfile profile, string dashboardRoute = DefaultDashboardRoute)
    {
        ArgumentNullException.ThrowIfNull(profile);
        CurrentProfile = profile;
        CurrentDashboardRoute = string.IsNullOrWhiteSpace(dashboardRoute)
            ? DefaultDashboardRoute
            : dashboardRoute;
    }

    public void SetDashboardRoute(string dashboardRoute)
    {
        CurrentDashboardRoute = string.IsNullOrWhiteSpace(dashboardRoute)
            ? DefaultDashboardRoute
            : dashboardRoute;
    }

    public void Clear()
    {
        CurrentProfile = null;
        CurrentDashboardRoute = DefaultDashboardRoute;
    }
}
