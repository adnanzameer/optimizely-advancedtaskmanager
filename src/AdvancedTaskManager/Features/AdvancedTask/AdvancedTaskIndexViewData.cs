using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvancedTaskManager.Infrastructure.Configuration;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer.DataAbstraction;

namespace AdvancedTaskManager.Features.AdvancedTask
{
    public class AdvancedTaskIndexViewData
    {
        public AdvancedTaskIndexViewData(List<LanguageBranchOption> languageBranchList, AdvancedTaskManagerOptions configuration, List<SiteOption> siteList = null)
        {
            LanguageBranchList = languageBranchList;
            SiteList = siteList ?? new List<SiteOption>();

            SelectedLanguageText = "Select";

            if (languageBranchList != null && languageBranchList.Any())
            {
                var selectedLanguage = languageBranchList.FirstOrDefault(x => x.Selected);

                if (selectedLanguage?.Language != null)
                {
                    SelectedLanguageText = selectedLanguage.Language.Name;
                }
            }

            HasPublishAccess = false;

            ContentTaskList = new List<ContentTask>();

            PageNumber = 1;

            AddContentApprovalDeadlineProperty = !configuration.DeleteContentApprovalDeadlineProperty && configuration.AddContentApprovalDeadlineProperty;

            PageSize = configuration.PageSize is > 1 and <= 200 ? configuration.PageSize : 30;

            DateTimeFormat = Extensions.TryGetValidDateFormat(configuration.DateTimeFormat) ?? "yyyy-MM-dd HH:mm";

            DateTimeFormatUserFriendly = Extensions.TryGetValidDateFormat(configuration.DateTimeFormatUserFriendly) ?? "MMM dd, yyyy, h:mm:ss tt";
        }

        public string DateTimeFormat { get; set; }

        public string DateTimeFormatUserFriendly { get; set; }

        // --- Filter state ---
        public string SelectedStatus { get; set; } = "inreview";
        public string SelectedContentType { get; set; } = "";
        public string SelectedSiteId { get; set; } = "";
        public string ContentFilterText { get; set; } = "";

        // --- Filter display text ---
        public string StatusDisplayText => SelectedStatus == "approved" ? "Ready to Publish" : "In Review";
        public string ContentTypeDisplayText => SelectedContentType switch
        {
            "page" => "Page",
            "block" => "Block",
            "asset" => "Asset / Media",
            _ => "All Types"
        };
        public string SiteDisplayText
        {
            get
            {
                var selected = SiteList.FirstOrDefault(s => s.Selected);
                return selected != null ? selected.SiteName : "All Sites";
            }
        }

        public IEnumerable<int> Pages
        {
            get
            {
                var list2 = new List<int> { 1 };
                var list = list2;
                if (PageNumber - PageSize - 1 > 1)
                {
                    list.Add(0);
                }
                for (var i = PageNumber - PageSize; i <= PageNumber + PageSize; i++)
                {
                    if (i > 1 && i < TotalPagesCount)
                    {
                        list.Add(i);
                    }
                }
                if (PageNumber + PageSize + 1 < TotalPagesCount)
                {
                    list.Add(0);
                }
                if (TotalPagesCount > 1)
                {
                    list.Add(TotalPagesCount);
                }
                return list;
            }
        }

        public int TotalPagesCount => (TotalItemsCount - 1) / PageSize + 1;

        public int MaxIndexOfItem
        {
            get
            {
                if (PageNumber * PageSize <= TotalItemsCount)
                {
                    return PageNumber * PageSize;
                }

                return TotalItemsCount;
            }
        }

        public int MinIndexOfItem
        {
            get
            {
                if (TotalItemsCount <= 0)
                {
                    return 0;
                }

                return (PageNumber - 1) * PageSize + 1;
            }
        }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public List<ContentTask> ContentTaskList { get; set; }

        public int TotalItemsCount { get; set; }

        public bool HasPublishAccess { get; set; }

        public string TaskValues { get; set; }

        public string QueryString { get; set; }

        public List<SiteOption> SiteList { get; set; }

        /// <summary>
        /// Replaces or adds a filter key in the query string and resets pagination to page 1.
        /// </summary>
        public string FilterUrl(string key, string value)
        {
            var qs = HttpUtility.ParseQueryString(QueryString);
            if (string.IsNullOrEmpty(value))
                qs.Remove(key);
            else
                qs[key] = value;
            qs.Remove("page");
            return $"?{qs}";
        }

        public string PageUrl(int page)
        {
            var qs = HttpUtility.ParseQueryString(QueryString);
            qs["page"] = page.ToString();
            return $"?{qs}";
        }

        public string LanguageUrl(string language)
        {
            var qs = HttpUtility.ParseQueryString(QueryString);
            qs["language"] = language;
            qs.Remove("page");
            return $"?{qs}";
        }

        /// <summary>
        /// Returns the current query string without the page parameter, for use with GetAllTaskIds.
        /// </summary>
        public string GetAllTasksQueryString()
        {
            var qs = HttpUtility.ParseQueryString(QueryString);
            qs.Remove("page");
            var result = qs.ToString();
            return string.IsNullOrEmpty(result) ? "" : "?" + result;
        }

        public bool HasActiveFilters =>
            SelectedStatus != "inreview"
            || !string.IsNullOrEmpty(SelectedContentType)
            || !string.IsNullOrEmpty(SelectedSiteId)
            || !string.IsNullOrEmpty(ContentFilterText);

        public bool AddContentApprovalDeadlineProperty { get; set; }

        public List<LanguageBranchOption> LanguageBranchList { get; set; }

        public string SelectedLanguageText { get; set; }
    }

    public class LanguageBranchOption
    {
        public LanguageBranch Language { get; set; }
        public bool Selected { get; set; }
    }

    public class SiteOption
    {
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public bool Selected { get; set; }
    }
}
