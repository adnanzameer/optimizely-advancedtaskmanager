using FoundationCore.Web.Models.Pages;

namespace FoundationCore.Web.Models.ViewModels;

/// <summary>
/// Defines common characteristics for view models for pages, including properties used by layout files.
/// </summary>
/// <remarks>
/// Views which should handle several page types (T) can use this interface as model type rather than the
/// concrete PageViewModel class, utilizing the that this interface is covariant.
/// </remarks>
public interface IPageViewModel<out T> where T : SitePageData
{
    T CurrentPage { get; }
    MetaDataModel MetaData { get; set; }
    LayoutModel Layout { get; set; }
    SettingsPage SettingsPage { get; set; }
    IContent Section { get; set; }
}
