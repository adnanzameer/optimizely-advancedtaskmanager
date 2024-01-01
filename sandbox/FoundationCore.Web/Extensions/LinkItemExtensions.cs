using EPiServer.SpecializedProperties;
using EPiServer.Web.Routing;

namespace FoundationCore.Web.Extensions
{
    public static class LinkItemExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Checks whether a LinkItem refers to an internal object.
        /// </summary>
        /// <param name="linkItem"></param>
        /// <returns></returns>
        public static bool IsInternal(this LinkItem linkItem)
        {
            if (linkItem == null)
                throw new ArgumentNullException(nameof(linkItem), "LinkItem cannot be null");

            return linkItem.ToContentReference() != ContentReference.EmptyReference || linkItem.Href.StartsWith("~/link", StringComparison.InvariantCultureIgnoreCase) ||
                   linkItem.Href.EndsWith("CMS/edit/PreviewContainerPage.aspx", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Returns a ContentReference for a LinkItem.
        /// </summary>
        /// <param name="linkItem"></param>
        /// <returns></returns>
        public static ContentReference ToContentReference(this LinkItem linkItem)
        {
            if (linkItem == null)
                return ContentReference.EmptyReference;

            var content = UrlResolver.Current.Route(new UrlBuilder(linkItem.Href));
            return content != null ? content.ContentLink : ContentReference.EmptyReference;
        }

        #endregion
    }
}
