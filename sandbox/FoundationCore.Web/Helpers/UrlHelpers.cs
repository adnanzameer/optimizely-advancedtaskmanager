using System.Globalization;
using EPiServer.Web;
using EPiServer.Web.Internal;
using EPiServer.Web.Routing;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Helpers
{
    public class UrlHelpers : IUrlHelpers
    {
        private readonly ISiteDefinitionResolver _siteDefinitionResolver;
        private readonly IUrlResolver _urlResolver;

        public UrlHelpers(IUrlResolver urlResolver, ISiteDefinitionResolver siteDefinitionResolver)
        {
            _urlResolver = urlResolver;
            _siteDefinitionResolver = siteDefinitionResolver;
        }

        public string ExternalUrl(ContentReference contentLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
                return string.Empty;

            return ContentExternalUrl(contentLink, true);
        }

        public string ContentExternalUrl(ContentReference contentLink, bool absoluteUrl)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
                return string.Empty;

            var result = _urlResolver.GetUrl(
                contentLink,
                CultureInfo.InvariantCulture.Name,
                new VirtualPathArguments
                {
                    ContextMode = ContextMode.Default,
                    ForceCanonical = absoluteUrl
                });

            if (absoluteUrl && Uri.TryCreate(result, UriKind.RelativeOrAbsolute, out var relativeUri) && !relativeUri.IsAbsoluteUri)
            {

                var siteDefinition = _siteDefinitionResolver.GetByContent(contentLink, true, true);
                var baseUri = siteDefinition.SiteUrl;

                if (baseUri != null)
                {
                    var absoluteUri = new Uri(baseUri, relativeUri);

                    return absoluteUri.AbsoluteUri;
                }
            }

            return result;
        }

        public string ContentUrl(ContentReference contentLink, string language = null)
        {

            if (!ContentReference.IsNullOrEmpty(contentLink))
            {
                var url = string.IsNullOrEmpty(language) ? _urlResolver.GetUrl(contentLink) : _urlResolver.GetUrl(contentLink, language);
                if (!string.IsNullOrEmpty(url))
                    return UrlEncoder.Encode(url);
            }

            return string.Empty;
        }

        public string ContentUrlWithoutLanguage(ContentReference contentLink)
        {

            if (!ContentReference.IsNullOrEmpty(contentLink))
            {
                var url = _urlResolver.GetUrl(contentLink);
                if (!string.IsNullOrEmpty(url))
                    return UrlEncoder.Encode(url);
            }

            return string.Empty;
        }

        public string GetExternalUrl(ContentReference contentReference, string language = "en")
        {

            var internalUrl = _urlResolver.GetUrl(contentReference, language);

            var url = new UrlBuilder(internalUrl);

            return UriSupport.AbsoluteUrlBySettings(url.ToString());
        }
    }
}