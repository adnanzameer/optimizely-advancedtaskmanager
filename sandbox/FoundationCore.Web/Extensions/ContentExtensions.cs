using EPiServer.Filters;
using EPiServer.Framework.Web;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace FoundationCore.Web.Extensions
{
    public static class ContentExtensions
    {
        #region Public Methods

        public static LinkItemCollection FilterForDisplay(this LinkItemCollection linkItemCollection, bool excludeExternal = false)
        {
            var newList = new LinkItemCollection();
            if (linkItemCollection == null)
                return newList;

            foreach (var linkItem in linkItemCollection)
            {
                var isInternal = linkItem.IsInternal();
                if (!isInternal && !excludeExternal)
                {
                    newList.Add(linkItem);
                    continue;
                }

                var contentReference = linkItem.ToContentReference();

                if (!ContentReference.IsNullOrEmpty(contentReference))
                {
                    ServiceLocator.Current.GetInstance<IContentLoader>().TryGet(contentReference, out IContent content);

                    content = content.FilterForDisplay(true);

                    if (content != null)
                        newList.Add(linkItem);
                }
            }

            return newList;
        }

        public static T FilterForDisplay<T>(this T content, bool requirePageTemplate = false, bool requireVisibleInMenu = false) where T : IContent
        {
            var accessFilter = new FilterAccess();
            var publishedFilter = new FilterPublished();
            content = !publishedFilter.ShouldFilter(content) && !accessFilter.ShouldFilter(content) ? content : default;
            if (content == null) return default;
            if (requirePageTemplate)
            {
                var templateFilter = ServiceLocator.Current.GetInstance<FilterTemplate>();
                templateFilter.TemplateTypeCategories = TemplateTypeCategories.Request;
                content = !templateFilter.ShouldFilter(content) && content.ParentLink != ContentReference.WasteBasket ? content : default;
            }

            if (requireVisibleInMenu)
            {
                content = VisibleInMenu(content) ? content : default;
            }

            if (content != null)
                return content;

            return default;
        }

        public static IEnumerable<T> FilterForDisplay<T>(this IEnumerable<T> contents, bool requirePageTemplate = false, bool requireVisibleInMenu = false)
            where T : IContent
        {
            var accessFilter = new FilterAccess();
            var publishedFilter = new FilterPublished();
            contents = contents.Where(x => !publishedFilter.ShouldFilter(x) && !accessFilter.ShouldFilter(x));
            if (requirePageTemplate)
            {
                var templateFilter = ServiceLocator.Current.GetInstance<FilterTemplate>();
                templateFilter.TemplateTypeCategories = TemplateTypeCategories.Request;
                contents = contents.Where(x => !templateFilter.ShouldFilter(x));
            }
            if (requireVisibleInMenu)
            {
                contents = contents.Where(x => VisibleInMenu(x));
            }
            return contents;
        }

        public static IEnumerable<PageReference> GetReferencesToContent(this IContent content)
        {
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var references = repository.GetReferencesToContent(content.ContentLink, false);

            foreach (var referenceInformation in references)
            {
                var result = repository.TryGet(referenceInformation.OwnerID, out PageData page);

                if (result)
                {
                    yield return page.PageLink;
                }

                repository.TryGet(referenceInformation.OwnerID, out BlockData blockData);

                if (blockData != null)
                {
                    var pageReferences = (blockData as IContent).GetReferencesToContent();

                    foreach (var reference in pageReferences)
                    {
                        yield return reference;
                    }
                }
            }
        }

        public static DateTime GetModifiedDate(this IContent content) =>
            ((IChangeTrackable)content).Saved;

        public static IEnumerable<IContent> ApplyPropertyFilter(this IEnumerable<IContent> enumerable, string propertyName, string propertyValue)
        {
            if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
            {
                return enumerable.Where(t => t.Property.Contains(propertyName)
                                             && t.Property[propertyName]?.Value != null
                                             && t.Property[propertyName].Value.ToString().ToLower().Contains(propertyValue.ToLower()));
            }

            return enumerable;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Checks if a content item is visible in the menu. Must be of type PageData
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static bool VisibleInMenu(IContent content)
        {
            var page = content as PageData;
            return page == null || page.VisibleInMenu;
        }

        #endregion
    }
}
