using System.ComponentModel.DataAnnotations;

namespace FoundationCore.Web;

public class Globals
{
    public const string LoginPath = "/util/login";
    public const string AppDataFolderName = "App_Data";

    /// <summary>
    /// Group names for content types and properties
    /// </summary>
    [GroupDefinitions]
    public static class GroupNames
    {
        [Display(Name = "Default", Order = 2)]
        public const string Default = "Default";

        [Display(Name = "Tracking", Order = 11)]
        public const string Tracking = "Tracking";

        [Display(Name = "Specialized", Order = 11)]
        public const string Specialized = "Specialized";

        [Display(Name = "Header", Order = 12)]
        public const string HeaderSettings = "Header";

        [Display(Name = "Footer", Order = 13)]
        public const string FooterSettings = "Footer";

        [Display(Name = "Search", Order = 14)]
        public const string Search = "Search";

        [Display(Name = "Partial", Order = 14)]
        public const string Partial = "Partial";

        [Display(Name = "SiteSettings", Order = 20)]
        public const string SiteSettings = "SiteSettings";

        [Display(Name = "Metadata", Order = 27)]
        public const string MetaData = "Metadata";

        [Display(Name = "Migration", Order = 28)]
        public const string Migration = "Migration";

        [Display(Name = "Teaser", Order = 50)]
        public const string Teaser = "Teaser";

        [Display(Name = "API Keys", Order = 29)]
        public const string ApiKeys = "API Keys";

        [Display(Name = "Forms", Order = 29)]
        public const string Forms = "Forms";
    }

    /// <summary>
    /// Tags to use for the main widths used in the Bootstrap HTML framework
    /// </summary>
    public static class ContentAreaTags
    {
        public const string FullWidth = "col-sm-12";
        public const string TwoThirdsWidth = "col-sm-8";
        public const string HalfWidth = "col-sm-6";
        public const string OneThirdsWidth = "col-sm-4";
        public const string OneFourthsWidth = "col-sm-3";
        public const string ThreeFourthsWidth = "col-sm-9";
        public const string NoRenderer = "";
    }

    /// <summary>
    /// Names used for UIHint attributes to map specific rendering controls to page properties
    /// </summary>
    public static class SiteUIHints
    {
        public const string Contact = "contact";
        public const string Strings = "StringList";
        public const string StringsCollection = "StringsCollection";
        public const string SanitizedXhtmlString = "SanitizedXhtmlString";
    }

    /// <summary>
    /// Virtual path to folder with static graphics, such as "/gfx/"
    /// </summary>
    public const string StaticGraphicsFolderPath = "/gfx/";
}
