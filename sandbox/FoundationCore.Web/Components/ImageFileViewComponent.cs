using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using FoundationCore.Web.Models.Media;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoundationCore.Web.Components;

/// <summary>
/// Controller for the image file.
/// </summary>
public class ImageFileViewComponent : AsyncPartialContentComponent<ImageFile>
{
    private readonly UrlResolver _urlResolver;

    public ImageFileViewComponent(UrlResolver urlResolver)
    {
        _urlResolver = urlResolver;
    }

    /// <summary>
    /// The index action for the image file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current image file.</param>
    protected override async Task<IViewComponentResult> InvokeComponentAsync(ImageFile currentContent)
    {
        var model = new ImageViewModel
        {
            Url = _urlResolver.GetUrl(currentContent.ContentLink),
            Name = currentContent.Name,
            Copyright = currentContent.Copyright
        };

        return await Task.FromResult(View(model));
    }
}
