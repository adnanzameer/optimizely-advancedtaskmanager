namespace FoundationCore.Web.Models;

/// <summary>
/// Attribute used for site content types to set default attribute values
/// </summary>
public class SiteContentTypeAttribute : ContentTypeAttribute
{
    public SiteContentTypeAttribute()
    {
        GroupName = Globals.GroupNames.Default;
    }
}
