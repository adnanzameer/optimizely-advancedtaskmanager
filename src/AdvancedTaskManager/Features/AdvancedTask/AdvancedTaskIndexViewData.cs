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
        public AdvancedTaskIndexViewData(List<LanguageBranchOption> languageBranchList, AdvancedTaskManagerOptions configuration)
        {
            LanguageBranchList = languageBranchList;
            
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
            return $"?{qs}";
        }

        public bool AddContentApprovalDeadlineProperty { get; set; }

        public List<LanguageBranchOption> LanguageBranchList { get; set; }

        public string SelectedLanguageText { get; set; }
    }

    public class LanguageBranchOption
    {
        public LanguageBranch Language { get; set; }
        public bool Selected { get; set; }
    }
}