using FoundationCore.Web.Models.Interface;



namespace FoundationCore.Web.Models.Pages;

/// <summary>
/// Used to logically group pages in the page tree
/// </summary>
[SiteContentType(GUID = "D178950C-D20E-4A46-90BD-5338B2424745", GroupName = Globals.GroupNames.Specialized, Order = 230)]
[AvailableContentTypes(Availability.Specific, Exclude = new[] { typeof(IIgnorable) })]
public class ContainerPage : PageData, IContainerPage, ISitePages
{
}
