namespace FoundationCore.Web.Models.Interface
{
    public interface IUrlHelpers
    {
        string ExternalUrl(ContentReference contentLink);
        string ContentExternalUrl(ContentReference contentLink, bool absoluteUrl);
        string ContentUrl(ContentReference contentLink, string language = null);
        string ContentUrlWithoutLanguage(ContentReference contentLink);
        string GetExternalUrl(ContentReference contentReference, string language = "en");
    }
}