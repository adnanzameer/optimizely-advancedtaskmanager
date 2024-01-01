using System.ComponentModel.DataAnnotations;
using EPiServer.Find.Json;


namespace FoundationCore.Web.Models.Blocks
{
    [SiteContentType(GUID = "01f9fa27-2b17-4e7a-8485-1a852de945e1", GroupName = Globals.GroupNames.Specialized, Order = 600)]
    public class TinyMCEBlock : SiteBlockData
    {
        [Display(
            Name = "Content",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        [CultureSpecific]
        
        public virtual XhtmlString Content { get; set; }

        [RemoveHtmlTagsWhenIndexing]
        public string SearchText => Content?.ToHtmlString() ?? string.Empty;

    }
}
