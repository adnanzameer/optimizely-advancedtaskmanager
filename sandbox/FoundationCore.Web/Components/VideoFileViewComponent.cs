using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using FoundationCore.Web.Models.Media;
using FoundationCore.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FoundationCore.Web.Components;

/// <summary>
/// Controller for the video file.
/// </summary>
public class VideoFileViewComponent : AsyncPartialContentComponent<VideoFile>
{
    private readonly UrlResolver _urlResolver;

    public VideoFileViewComponent(UrlResolver urlResolver)
    {
        _urlResolver = urlResolver;
    }

    /// <summary>
    /// The index action for the video file. Creates the view model and renders the view.
    /// </summary>
    /// <param name="currentContent">The current video file.</param>
    protected override async Task<IViewComponentResult> InvokeComponentAsync(VideoFile currentContent)
    {
        var model = new VideoViewModel
        {
            Url = _urlResolver.GetUrl(currentContent.ContentLink),
            PreviewImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage)
                ? null
                : _urlResolver.GetUrl(currentContent.PreviewImage),
        };

        return await Task.FromResult(View(model));
    }
}
