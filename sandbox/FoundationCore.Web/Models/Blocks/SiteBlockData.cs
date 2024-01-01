using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Models.Blocks;

/// <summary>
/// Base class for all block types on the site
/// </summary>
public abstract class SiteBlockData : BlockData, ICustomCssInContentArea
{
 [Ignore]
    public virtual string ContentAreaCssClass => "";

    public override void SetDefaultValues(ContentType contentType)
    {
        base.SetDefaultValues(contentType);
    }
}
