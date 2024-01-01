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


    //[CultureSpecific]
    //[Display(GroupName = Globals.GroupNames.Forms, Order = 70)]
    //[EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<CustomFormEmailTemplateItem>))]
    //[FriendlyPropertyListUrl(nameof(StoryTeaserItem.Hyperlink), nameof(StoryTeaserItem.Image))]
    //[HtmlViewerExtender(nameof(CustomFormEmailTemplateItem.Body))]
    //[ClientEditor(ClientEditingClass = "feller/editors/ExtendedCollectionEditor")]
    //public virtual IList<CustomFormEmailTemplateItem> FormMessageTemplate { get; set; }
}
