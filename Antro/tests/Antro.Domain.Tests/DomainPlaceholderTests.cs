using Antro.Domain;

namespace Antro.Domain.Tests;

public sealed class DomainPlaceholderTests
{
    [Fact]
    public void Placeholder_has_expected_default_name()
    {
        var placeholder = new DomainPlaceholder();

        Assert.Equal("Antro.Domain", placeholder.Name);
    }
}
