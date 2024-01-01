using EPiServer.Web.Routing;
using FoundationCore.Web.Extensions;
using FoundationCore.Web.Helpers;
using FoundationCore.Web.Models.Interface;
using FoundationCore.Web.Models.Pages;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoundationCore.Web.Business;

/// <summary>
/// Intercepts actions with view models of type IPageViewModel and populates the view models
/// Layout and Section properties.
/// </summary>
/// <remarks>
/// This filter frees controllers for pages from having to care about common context needed by layouts
/// and other page framework components allowing the controllers to focus on the specifics for the page types
/// and actions that they handle.
/// </remarks>
public class PageContextActionFilter : IResultFilter
{
    private readonly PageViewContextFactory _contextFactory;

    public PageContextActionFilter(PageViewContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        var controller = context.Controller as Controller;
        var viewModel = controller?.ViewData.Model;
        var epiEditMode = ViewContextExtension.IsInEditMode();

        if (viewModel is not IPageViewModel<SitePageData> model)
            return;

        var currentContentLink = context.HttpContext.GetContentLink();

        var metadataModel = model.MetaData ?? _contextFactory.CreateMetaDataModel(model.CurrentPage);
        if (context.Controller is IModifyMetaData metadataController)
        {
            metadataController.ModifyMetaData(metadataModel);
        }
        model.MetaData = metadataModel;

        var layoutModel = model.Layout ?? _contextFactory.CreateLayoutModel(currentContentLink, context.HttpContext);

        if (context.Controller is IModifyLayout layoutController)
        {
            layoutController.ModifyLayout(layoutModel);
        }

        model.Layout = layoutModel;

        model.Section ??= _contextFactory.GetSection(currentContentLink);

        model.SettingsPage = PageHelper.SiteSettingsPage;
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        //Required
    }
}
