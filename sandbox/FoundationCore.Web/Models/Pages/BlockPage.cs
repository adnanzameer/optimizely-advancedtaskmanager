using System.ComponentModel.DataAnnotations;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using FoundationCore.Web.Models.Interface;



namespace FoundationCore.Web.Models.Pages;

[ContentType(GUID = "e1ddc9e5-59cb-466f-ad5f-3ad3ec732d2c", GroupName = Globals.GroupNames.Default, Order = 220)]
[AvailableContentTypes(Availability.Specific, Include = new[] { typeof(ISitePages) }, Exclude = new[] { typeof(IIgnorable) })]
public class BlockPage : SitePageData, ISearchableContent, ISitePages
{
    [Display(
        GroupName = SystemTabNames.Content,
        Order = 320)]
    [CultureSpecific]
    public virtual ContentArea MainContentArea { get; set; }

    #region SearchProperties
    public IDictionary<string, IndexValue> SearchMetaData
    {
        get
        {
            var thumbnail = "";
            var reference = !ContentReference.IsNullOrEmpty(ThumbnailImage)
                ? ThumbnailImage
                : ContentReference.EmptyReference;
            if (!ContentReference.IsNullOrEmpty(reference))
            {
                var urlResolver = ServiceLocator.Current.GetInstance<IUrlHelpers>();
                thumbnail = urlResolver.ExternalUrl(reference);
            }
            var altText = !ContentReference.IsNullOrEmpty(ThumbnailImage) ? ThumbnailImageAltText : "";
            var dictionary = new Dictionary<string, IndexValue>
            {
                {"thumbnail", thumbnail},
                {"alttext", altText},
            };

            return dictionary;
        }
    }
    #endregion
}
