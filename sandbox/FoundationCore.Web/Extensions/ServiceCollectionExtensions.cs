using EPiServer.Web;
using FoundationCore.Web.Business;
using FoundationCore.Web.Business.Channels;
using FoundationCore.Web.Business.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace FoundationCore.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFoundationCore(this IServiceCollection services)
    {
        services.Configure<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new SiteViewEngineLocationExpander()));

        services.Configure<DisplayOptions>(displayOption =>
        {
            displayOption.Add("full", "/displayoptions/full", Globals.ContentAreaTags.FullWidth, string.Empty, "epi-icon__layout--full");
        });

        services.Configure<MvcOptions>(options => options.Filters.Add<PageContextActionFilter>());

        services.AddDisplayResolutions();
        services.AddDetection();

        return services;
    }

    private static void AddDisplayResolutions(this IServiceCollection services)
    {
        services.AddSingleton<StandardResolution>();
        services.AddSingleton<IpadHorizontalResolution>();
        services.AddSingleton<IphoneVerticalResolution>();
        services.AddSingleton<AndroidVerticalResolution>();
    }

}
