using Antro.Application;

namespace Antro.Application.Tests;

public sealed class ApplicationPlaceholderTests
{
    [Fact]
    public void Placeholder_exposes_application_marker()
    {
        var placeholder = new ApplicationPlaceholder();

        Assert.Equal("Antro.Application", placeholder.Name);
    }
}
