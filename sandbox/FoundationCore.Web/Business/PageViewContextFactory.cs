using EPiServer.Data;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using FoundationCore.Web.Models.Pages;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FoundationCore.Web.Business;

[ServiceConfiguration]
public class PageViewContextFactory
{
    private readonly IContentLoader _contentLoader;
    private readonly UrlResolver _urlResolver;
    private readonly IDatabaseMode _databaseMode;
    private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;

    public PageViewContextFactory(
        IContentLoader contentLoader,
        UrlResolver urlResolver,
        IDatabaseMode databaseMode,
        IOptionsMonitor<CookieAuthenticationOptions> optionMonitor)
    {
        _contentLoader = contentLoader;
        _urlResolver = urlResolver;
        _databaseMode = databaseMode;
        _cookieAuthenticationOptions = optionMonitor.Get(IdentityConstants.ApplicationScheme);
    }

    public virtual MetaDataModel CreateMetaDataModel(SitePageData currentPage)
    {

        if (currentPage == null)
            return new MetaDataModel();

        return new MetaDataModel
        {
            CanonicalLink = currentPage.CanonicalLink,
            DisableIndexing = currentPage.DisableIndexing,
            DoNotCachePage = currentPage.DoNotCachePage,
            MetaDescription = currentPage.MetaDescription,
            MetaKeywords = currentPage.MetaKeyword,
            MetaTitle = currentPage.MetaTitle,
            NoFollow = currentPage.NoFollow,
            NoIndex = currentPage.NoIndex,
        };
    }

    public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, HttpContext httpContext)
    {
        return new LayoutModel
        {
            LogotypeLinkUrl = new HtmlString(_urlResolver.GetUrl(SiteDefinition.Current.StartPage)),
            LoggedIn = httpContext.User.Identity != null && httpContext.User.Identity.IsAuthenticated,
            LoginUrl = new HtmlString(GetLoginUrl(currentContentLink)),
            IsInReadonlyMode = _databaseMode.DatabaseMode == DatabaseMode.ReadOnly
        };
    }

    private string GetLoginUrl(ContentReference returnToContentLink)
    {
        return $"{_cookieAuthenticationOptions?.LoginPath.Value ?? Globals.LoginPath}?ReturnUrl={_urlResolver.GetUrl(returnToContentLink)}";
    }

    public virtual IContent GetSection(ContentReference contentLink)
    {
        if (contentLink == null)
            return default;

        var currentContent = _contentLoader.Get<IContent>(contentLink);

        static bool IsSectionRoot(ContentReference contentReference) =>
           ContentReference.IsNullOrEmpty(contentReference) ||
           contentReference.Equals(SiteDefinition.Current.StartPage) ||
           contentReference.Equals(SiteDefinition.Current.RootPage);

        if (IsSectionRoot(currentContent.ParentLink))
        {
            return currentContent;
        }

        return _contentLoader.GetAncestors(contentLink)
            .OfType<PageData>()
            .SkipWhile(x => !IsSectionRoot(x.ParentLink))
            .FirstOrDefault();
    }
}
