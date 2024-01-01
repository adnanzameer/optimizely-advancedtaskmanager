using System.ComponentModel.DataAnnotations;
using FoundationCore.Web.Models.CustomProperties;

namespace FoundationCore.Web.Models.Blocks.Properties
{
    [ContentType(Order = 110, GUID = "4b4b22c0-10cf-4982-a710-fd9f62fef201", AvailableInEditMode = false)]
    public class CtaBlock : BlockData
    {
        [CultureSpecific]
        [Display(Order = 10)]
        public virtual string Title { get; set; }

        [CultureSpecific]
        [Display(Order = 20)]
        public virtual string LinkText { get; set; }

        [CultureSpecific]
        [Display(Order = 30)]
        [BackingType(typeof(PropertyLongUrl))]
        public virtual Url Hyperlink { get; set; }

        [UIHint("TargetFrame")]
        [Display(Order = 40)]
        public virtual int? TargetFrameRaw { get; set; }

        [Display(Order = 78)]
        public virtual bool NoFollow { get; set; }

        [Ignore]
        public string TargetFrame => TargetFrameRaw is 1 ? "_blank" : "_self";
    }
}