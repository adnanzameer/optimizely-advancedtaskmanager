namespace FoundationCore.Web.Models.ViewModels
{
    public class MetaDataModel
    {
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }

        public string MetaKeywords { get; set; }

        public string CanonicalLink { get; set; }

        public string SEOSitemaps { get; set; }

        public bool DisableIndexing { get; set; }

        public bool NoIndex { get; set; }

        public bool NoFollow { get; set; }

        public bool DoNotCachePage { get; set; }
    }
}
