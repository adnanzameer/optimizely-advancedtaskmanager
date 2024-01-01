using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using EPiServer.Data;
using EPiServer.Find;
using EPiServer.Forms;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using FoundationCore.Web.Models.Interface;
using FoundationCore.Web.Models.Pages;

namespace FoundationCore.Web.Helpers
{
    public static class PageHelper
    {
        private static readonly Injected<IContentLoader> ContentLoader;
        private static readonly Injected<IDatabaseMode> DatabaseMode;
        private static readonly Injected<IHttpContextAccessor> HttpContextAccessor;
        private static readonly Injected<IContextModeResolver> ContextModeResolver;
        private static readonly Injected<ISiteDefinitionResolver> SiteDefinitionResolver;
        private static readonly Injected<IUrlHelpers> UrlHelpers;
        private const string DefaultCulture = "en";

        public static bool IsDatabaseInReadOnlyMode => DatabaseMode.Service.DatabaseMode == EPiServer.Data.DatabaseMode.ReadOnly;
        public static SettingsPage SiteSettingsPage
        {
            get
            {
                try
                {
                    var settingPage = !ContentReference.IsNullOrEmpty(ContentReference.StartPage)
                        ? ContentLoader.Service.GetChildren<SettingsPage>(ContentReference.StartPage).SingleOrDefault()
                        : null;
                    return settingPage;
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger().Error(ex.Message, ex);
                    return ContentLoader.Service.Get<SettingsPage>(new ContentReference(7));// CMS ID
                }
            }
        }

        public static SettingsPage SiteSettingsPageByLanguageCode(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                languageCode = DefaultCulture;
            }
            var loadingOptions = new LoaderOptions { LanguageLoaderOption.FallbackWithMaster(GetCultureInfo(languageCode)) };

            var settingPage = !ContentReference.IsNullOrEmpty(ContentReference.StartPage)
                ? ContentLoader.Service.GetChildren<SettingsPage>(ContentReference.StartPage, loadingOptions).SingleOrDefault()
                : null;
            return settingPage;

        }

        /// <summary>
        /// Get <see cref="CultureInfo"/> object using the <paramref name="languageCode"/>.
        /// </summary>
        /// <param name="languageCode">Language code (<see cref="CultureInfo.Name"/>).</param>
        /// <returns><see cref="CultureInfo"/> object created using the <paramref name="languageCode"/> or if there was an exception then a CultureInfo object is returned using  the <see cref="Constants.DefaultCulture"/>.</returns>
        public static CultureInfo GetCultureInfo(string languageCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(languageCode))
                {
                    var culture = CultureInfo.GetCultureInfo(languageCode);
                    if (!culture.NativeName.Contains("Unknown Language")) return culture;

                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(ex.Message, ex);
            }

            // this is fallback if there was an exception or the languageCode was null
            return CultureInfo.GetCultureInfo(DefaultCulture);
        }


        public static XhtmlString AddLazyImages(this XhtmlString xhtmlString)
        {
            if (PageIsInEditMode())
            {
                return xhtmlString;
            }

            var processedText = Regex.Replace(
                xhtmlString.ToInternalString(),
                "(<img)",
                "$1 loading=\"lazy\""
            );

            return new XhtmlString(processedText);
        }

        public static string GetCanonicalUrl(ContentReference contentLink, string canonicalLink)
        {
            var siteDefinition = SiteDefinitionResolver.Service.GetByContent(contentLink, true, true);
            var baseUri = siteDefinition.SiteUrl;

            if (!string.IsNullOrEmpty(canonicalLink))
            {
                var customUrl = GetCustomUrl(baseUri, canonicalLink);

                if (customUrl.EndsWith("/"))
                {
                    customUrl = customUrl.Remove(customUrl.Length - 1, 1);
                }

                return customUrl;

            }

            var friendlyUrl = UrlHelpers.Service.ExternalUrl(contentLink);

            if (string.IsNullOrEmpty(friendlyUrl) && HttpContextAccessor.Service.HttpContext != null)
            {
                friendlyUrl = GetFriendlyUrl(baseUri, HttpContextAccessor.Service.HttpContext.Request.Host);
            }

            if (friendlyUrl != null)
            {
                friendlyUrl = friendlyUrl.Replace("http://", "https://");

                if (friendlyUrl.EndsWith("/"))
                {
                    friendlyUrl = friendlyUrl.Remove(friendlyUrl.Length - 1, 1);
                }

                return friendlyUrl;
            }

            return "";
        }

        private static string GetCustomUrl(Uri baseUri, string canonicalLink)
        {
            string customUrl;
            if (canonicalLink.StartsWith("http", StringComparison.Ordinal))
            {
                customUrl = canonicalLink;
            }
            else
            {
                var canonicalUrl = canonicalLink;

                if (!canonicalUrl.StartsWith("/"))
                {
                    canonicalUrl = "/" + canonicalUrl;
                }

                var absoluteUri = new Uri(baseUri, canonicalUrl);

                customUrl = absoluteUri.AbsoluteUri;
            }

            if (customUrl.EndsWith("/"))
            {
                customUrl = customUrl.Remove(customUrl.Length - 1, 1);
            }

            return customUrl;
        }

        private static string GetFriendlyUrl(Uri baseUri, HostString hostString)
        {
            var friendlyUrl = baseUri != null
                ? baseUri.AbsolutePath
                : "https://" + hostString;

            if (friendlyUrl.EndsWith("/"))
            {
                friendlyUrl = friendlyUrl.Remove(friendlyUrl.Length - 1, 1);
            }

            return friendlyUrl;
        }

        public static bool PageIsInEditMode() => ContextModeResolver.Service.CurrentMode == ContextMode.Edit;

        public static bool PageIsInPreviewMode() => ContextModeResolver.Service.CurrentMode == ContextMode.Preview;

        public static string Sanitize(this string dirtyString)
        {
            if (string.IsNullOrEmpty(dirtyString))
                return "";

            if (dirtyString.Length > 40)
            {
                dirtyString = dirtyString.Substring(0, 40);
            }

            
            return WebUtility.HtmlEncode(UseStringBuilderWithHashSet(dirtyString));
        }

        private static string UseStringBuilderWithHashSet(string dirtyString)
        {
            return Regex.Replace(dirtyString, "[^0-9A-Za-z ,]", "");

        }

        public static string GetValueFromSearchMetaData(IDictionary<string, IndexValue> metaData, string key)
        {
            try
            {
                if (metaData != null)
                {
                    metaData.TryGetValue(key, out var value);

                    if (value != null)
                    {
                        return value.StringValue;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error("Custom GetValueFromSearchMetaData", ex);
            }

            return "";
        }

        public static string FormatBytes(long bytes)
        {
            string[] uom = { "B", "KB", "MB", "GB" };

            int i;

            double size = bytes;

            for (i = 0; i < uom.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                size = bytes / 1024.0;
            }

            return $"{size:0.##} {uom[i]}";
        }
    }
}