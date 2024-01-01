using EPiServer.Web.Routing;

namespace FoundationCore.Web.Extensions
{
    public static class ContentRepositoryExtensions
    {
        private static readonly UrlResolver _urlResolver;

        static ContentRepositoryExtensions() =>
            _urlResolver = UrlResolver.Current;

        public static IEnumerable<string> GetUsageUrls(this IContentRepository contentRepository, IContent currentContent)
        {
            var result = new List<string>();

            var usageReferences = contentRepository.GetReferencesToContent(currentContent.ContentLink, false)?
                                                   .Select(x => x.OwnerID);

            if (usageReferences != null)
            {
                foreach (var reference in usageReferences)
                    result.Add(_urlResolver.GetUrl(reference));
            }

            //TODO: Need to show links for the multiple lang branches
            //var langIContents = contentRepository.GetLanguageBranches<IContent>(currentContent.ContentLink);

            //if (langIContents != null)
            //{
            //    foreach (var langIContent in langIContents)
            //        result.Add(_urlResolver.GetUrl(langIContent.ContentLink));
            //}

            return result;
        }
    }
}
