using System.ComponentModel.DataAnnotations;
using EPiServer.Web;
using FoundationCore.Web.Models.Blocks;
using FoundationCore.Web.Models.Interface;



namespace FoundationCore.Web.Models.Pages
{
    [ContentType(GUID = "9b0c3f6d-5b38-464f-b437-859c64a59ab9", GroupName = Globals.GroupNames.Specialized, Order = 270)]
    [AvailableContentTypes(Availability.Specific, Exclude = new[] { typeof(PageData) }, IncludeOn = new[] { typeof(IStartPageType) })]

    public class SettingsPage : PageData, IIgnorable, ISettingsPageType, ISitePages
    {
        #region Settings
        [Display(Order = 110, GroupName = SystemTabNames.Content)]
        public virtual SiteLogotypeBlock SiteLogotype { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 370)]
        public virtual PageReference SearchResultPageReference { get; set; }

        #endregion

        #region APiKeys

        [Display(GroupName = Globals.GroupNames.ApiKeys, Order = 350)]
        [CultureSpecific]
        [UIHint(UIHint.Textarea)]
        public virtual string GoogleTagManagerId { get; set; }

        [Display(GroupName = Globals.GroupNames.ApiKeys, Order = 370)]
        [UIHint(UIHint.Textarea)]
        public virtual string MailChimpApiKey { get; set; }
        #endregion

    }
}