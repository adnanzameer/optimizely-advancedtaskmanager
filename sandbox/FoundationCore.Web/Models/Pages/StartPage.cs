using System.ComponentModel.DataAnnotations;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Models.Pages;

[ContentType(GUID = "19671657-B684-4D95-A61F-8DD4FE60D559", GroupName = Globals.GroupNames.Specialized, Order = 228)]
[AvailableContentTypes(Availability.Specific, Include = new[] { typeof(ISitePages)})]
public class StartPage : SitePageData, IStartPageType
{
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 320)]
    [CultureSpecific]
    public virtual ContentArea MainContentArea { get; set; }
}
