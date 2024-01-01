using EPiServer.Globalization;
using FoundationCore.Web.Models.Pages;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoundationCore.Web.Controllers
{
    [Route("error")]
    public class ErrorPageController : PageControllerBase<ErrorPage>
    {
        private readonly IContentLoader _contentLoader;

        public ErrorPageController(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        [Route("404")]
        public ActionResult Index(ErrorPage currentPage)
        {
            if (currentPage == null)
                return NotFound();

            currentPage = GetLocalizedOrDefaultErrorPage(currentPage);

            var model = new PageViewModel<ErrorPage>(currentPage);

            return View(model);
        }

        private ErrorPage GetLocalizedOrDefaultErrorPage(ErrorPage errorPage)
        {
            //// Determine language of the NotFoundUrl
            //if (string.IsNullOrEmpty(ViewBag.NotFoundUrl))
            //	return errorPage;

            //var uri = new Uri(ViewBag.NotFoundUrl);

            //// if URI segments of the NotFoundUrl are composed of: 1) "/", 2) "{lang}/" (i.e. "/pl/incorrecturl")
            //// then load the correct language of the Error page
            //if (uri.Segments.Length <= 2 || uri.Segments[0] != "/" || !uri.Segments[1].EndsWith("/"))
            //	return errorPage;

            //var firstWordSegmentInUrl = uri.Segments[1].Substring(0, uri.Segments[1].Length - 1);

            // iterate over the existing languages for the error page
            // and check if any of the languages' URL segment matches the first segment of the accessed URL
            foreach (var language in errorPage.ExistingLanguages)
            {
                if (language.Name == ContentLanguage.PreferredCulture.Name)
                {
                    var errorPageInLanguage = _contentLoader.Get<ErrorPage>(errorPage.ContentLink, language);
                    if (errorPageInLanguage != null)
                    {
                        errorPage = errorPageInLanguage;
                        break;
                    }
                }
            }

            return errorPage;
        }
    }
}
