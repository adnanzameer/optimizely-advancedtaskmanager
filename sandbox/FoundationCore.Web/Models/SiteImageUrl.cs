namespace FoundationCore.Web.Models;

/// <summary>
/// Attribute to set the default thumbnail for site page and block types
/// </summary>
public class SiteImageUrlAttribute : ImageUrlAttribute
{
    /// <summary>
    /// The parameterless constructor will initialize a SiteImageUrl attribute with a default thumbnail
    /// </summary>
    public SiteImageUrlAttribute()
        : base("/gfx/page-type-thumbnail.png")
    {
    }

    public SiteImageUrlAttribute(string path)
        : base(path)
    {
    }
}
