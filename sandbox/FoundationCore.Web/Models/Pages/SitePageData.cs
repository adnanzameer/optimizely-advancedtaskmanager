using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer.Web;

namespace FoundationCore.Web.Models.Pages;

/// <summary>
/// Base class for all page types
/// </summary>
public abstract class SitePageData : PageData
{
    #region MetaData

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 20)]
    [CultureSpecific]
    [StringLength(70)]
    public virtual string MetaTitle
    {
        get
        {
            var metaTitle = this.GetPropertyValue(p => p.MetaTitle);

            // Use explicitly set meta title, otherwise fall back to page name
            return !string.IsNullOrWhiteSpace(metaTitle)
                ? metaTitle
                : PageName;
        }
        set { this.SetPropertyValue(p => p.MetaTitle, value); }
    }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 30)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string MetaKeyword { get; set; }

    [Ignore]
    [Searchable]
    public virtual string[] SearchableMetaKeywords => !string.IsNullOrEmpty(MetaKeyword) ? MetaKeyword.Split(',') : Array.Empty<string>();

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 40)]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    [StringLength(150)]
    public virtual string MetaDescription { get; set; }

    [UIHint(UIHint.Image)]
    [Display(GroupName = Globals.GroupNames.MetaData, Order = 42)]
    [CultureSpecific]
    public virtual ContentReference ThumbnailImage { get; set; }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 44)]
    public virtual string ThumbnailImageAltText { get; set; }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 50)]
    [CultureSpecific]
    public virtual string CanonicalLink { get; set; }

    [Display(GroupName = SystemTabNames.PageHeader, Order = 70)]
    [CultureSpecific]
    public virtual bool DisableIndexing { get; set; }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 80)]
    public virtual bool DoNotCachePage { get; set; }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 90)]
    public virtual bool NoIndex { get; set; }

    [Display(GroupName = Globals.GroupNames.MetaData, Order = 100)]
    public virtual bool NoFollow { get; set; }

    #endregion

    #region Settings
    [Display(GroupName = SystemTabNames.Settings, Order = 200)]
    [CultureSpecific]
    public virtual bool HideBreadcrumb { get; set; }

    [Display(GroupName = SystemTabNames.Settings, Order = 210)]
    [CultureSpecific]
    public virtual bool HideSiteHeader { get; set; }

    [Display(GroupName = SystemTabNames.Settings, Order = 220)]
    [CultureSpecific]
    public virtual bool HideSiteFooter { get; set; }


    #endregion

    #region SearchProperties

    public virtual DateTime? SearchPublishDate => StartPublish;

    public virtual string SearchTitle => MetaTitle;

    public virtual string SearchTypeName => !string.IsNullOrWhiteSpace(PageTypeName)
        ? PageTypeName.ToLowerInvariant()
        : GetType().Name.ToLowerInvariant();

    public virtual string SearchText => string.Format(CultureInfo.InvariantCulture, "{0}", MetaTitle);

    public virtual string SearchHitTypeName => "page";

    public virtual string SearchTypeNameAsSearchCategory =>
        SearchTypeName.EndsWith("page")
            ? SearchTypeName.Substring(0, SearchTypeName.Length - "page".Length)
            : SearchTypeName;

    #endregion
}
