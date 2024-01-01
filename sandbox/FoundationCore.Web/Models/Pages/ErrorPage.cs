using System.ComponentModel.DataAnnotations;
using FoundationCore.Web.Models.Blocks;
using FoundationCore.Web.Models.Interface;



namespace FoundationCore.Web.Models.Pages
{
    [ContentType(GUID = "0cbf9858-1d28-4220-8476-7dfdec461f52", GroupName = Globals.GroupNames.Specialized, Order = 240)]
    [AvailableContentTypes(Availability.Specific, Exclude = new[] { typeof(PageData) }, IncludeOn = new[] { typeof(IStartPageType) })]
    public class ErrorPage : SitePageData, IIgnorable, ISitePages
    {
        [Display(GroupName = SystemTabNames.Content, Order = 330)]
        [CultureSpecific]
        [AllowedTypes(typeof(SiteBlockData))]
        public virtual ContentArea MainContentArea { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            DisableIndexing = true;
            HideBreadcrumb = true;
            DoNotCachePage = true;
            VisibleInMenu = false;
            ExternalURL = "404";
        }

    }
}