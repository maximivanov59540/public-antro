using Antro.Content.Moscow2026;

namespace Antro.Content.Tests;

public sealed class Moscow2026CatalogPlaceholderTests
{
    [Fact]
    public void Placeholder_exposes_catalog_marker()
    {
        var placeholder = new Moscow2026CatalogPlaceholder();

        Assert.Equal("moscow-2026-placeholder", placeholder.CatalogId);
    }
}
