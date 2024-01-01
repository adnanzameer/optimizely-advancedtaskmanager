namespace FoundationCore.Web.Helpers
{
    public static class ContentAreaHelper
    {
        private static bool IsNullOrEmpty(this ContentArea contentArea)
        {
            return contentArea == null || !contentArea.FilteredItems.Any();
        }

        public static List<T> GetFilteredItemsOfType<T>(this ContentArea contentArea) where T : IContentData
        {
            var items = new List<T>();

            if (contentArea.IsNullOrEmpty())
            {
                return items;
            }

            foreach (var contentAreaItem in contentArea.FilteredItems)
            {
                if (contentAreaItem.LoadContent() is T content)
                {
                    items.Add(content);
                }
            }

            return items;
        }

    }
}
