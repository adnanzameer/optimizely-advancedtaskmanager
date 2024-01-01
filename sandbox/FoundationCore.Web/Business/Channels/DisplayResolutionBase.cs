using EPiServer.Framework.Localization;
using EPiServer.Web;

namespace FoundationCore.Web.Business.Channels;

/// <summary>
/// Base class for all resolution definitions
/// </summary>
public abstract class DisplayResolutionBase : IDisplayResolution
{
    private readonly LocalizationService _localizationService;
    protected DisplayResolutionBase(LocalizationService localizationService, string name, int width, int height)
    {
        _localizationService = localizationService;
        Id = GetType().FullName;
        Name = Translate(name);
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Gets the unique ID for this resolution
    /// </summary>
    public string Id { get; protected set; }

    /// <summary>
    /// Gets the name of resolution
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets the resolution width in pixels
    /// </summary>
    public int Width { get; protected set; }

    /// <summary>
    /// Gets the resolution height in pixels
    /// </summary>
    public int Height { get; protected set; }

    private string Translate(string resourceKey)
    {
        if (!_localizationService.TryGetString(resourceKey, out var value))
        {
            value = resourceKey;
        }

        return value;
    }
}
