using EPiServer.Web;
using FoundationCore.Web.Models.Pages;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoundationCore.Web.Controllers;

public class StartPageController : PageControllerBase<StartPage>
{
    public IActionResult Index(StartPage currentPage)
    {
        var model = PageViewModel.Create(currentPage);

        // Check if it is the StartPage or just a page of the StartPage type.
        if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink))
        {
            // Connect the view models logotype property to the start page's to make it editable
            //var editHints = ViewData.GetEditHints<PageViewModel<StartPage>, StartPage>();
            //editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
        }

        return View(model);
    }

    public override void ModifyLayout(LayoutModel layoutModel)
    {
        base.ModifyLayout(layoutModel);
        layoutModel.HideBreadcrumb = true;
    }

    //public void ModifyMetaData(MetaDataModel metaDataModel)
    //{
    //    metaDataModel.CanonicalLink = _canonicalUrl;
    //    metaDataModel.MetaTitle = _metaTitle;
    //    metaDataModel.MetaDescription = _metaDescription;
    //}
}
