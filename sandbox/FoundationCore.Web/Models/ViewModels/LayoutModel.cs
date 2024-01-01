using Microsoft.AspNetCore.Html;

namespace FoundationCore.Web.Models.ViewModels;

public class LayoutModel
{
    public IHtmlContent LogotypeLinkUrl { get; set; }

    public bool HideHeader { get; set; }

    public bool HideFooter { get; set; }

    public bool HideBreadcrumb { get; set; }

    public string PageContainerClass { get; set; }

    public bool LoggedIn { get; set; }

    public HtmlString LoginUrl { get; set; }

    public HtmlString LogOutUrl { get; set; }

    public bool IsInReadonlyMode { get; set; }
}
