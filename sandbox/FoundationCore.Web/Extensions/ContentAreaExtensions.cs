using EPiServer.Forms.Helpers.Internal;

namespace FoundationCore.Web.Extensions
{
    public static class ContentAreaExtensions
    {
        #region Public Methods

        public static List<T> GetContent<T>(this ContentArea contentArea, bool filterForDisplay = false)
        {
            if (contentArea?.Items != null && contentArea.Items.Any())
            {
                var contentItemlist = new List<T>();
                var itemsToEnumerate = filterForDisplay ? contentArea.FilteredItems : contentArea.Items;

                foreach (var contentAreaItem in itemsToEnumerate)
                {
                    if (contentAreaItem != null)
                    {
                        var contentItem = contentAreaItem.ContentLink.GetContent(false);
                        //only add to the list if returned item matches the type passed in.
                        if (contentItem is T contentItemOfType)
                            contentItemlist.Add(contentItemOfType);
                    }
                }

                if (contentItemlist.Any())
                    return contentItemlist;
            }

            return new List<T>(); //return empty list by default
        }

        public static List<IContent> GetContent(this ContentArea contentArea, bool filterForDisplay = false)
        {
            if (contentArea?.Items != null && contentArea.Items.Any())
            {
                var contentItemList = new List<IContent>();
                var itemsToEnumerate = filterForDisplay ? contentArea.FilteredItems : contentArea.Items;

                foreach (var contentAreaItem in itemsToEnumerate)
                {
                    var contentItem = contentAreaItem?.ContentLink.GetContent(false);
                    if (contentItem != null)
                        contentItemList.Add(contentItem);
                }

                if (contentItemList.Any())
                    return contentItemList;
            }

            return new List<IContent>(); //return empty list by default
        }

        public static bool HasContent(this ContentArea contentArea, bool filterForDisplay = false)
        {
            var hasContent = contentArea?.Items != null && contentArea.Items.Any(); //is the content are not null and does it have items

            if (hasContent && filterForDisplay)
                return contentArea.GetContent().FilterForDisplay().Any(); //check if there is any content after filtering
            return hasContent;
        }

        #endregion
    }
}
