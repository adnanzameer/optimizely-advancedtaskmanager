using System;
using EPiServer.Core;

namespace AdvancedTaskManager.Features.AdvancedTask
{
    public class ChangeTaskViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int ApprovalID { get; set; }

        public ContentReference AppliedOnContentLink
        {
            get => ContentReference.TryParse(AppliedOnContent, out var result) ? result : ContentReference.EmptyReference;
            set => AppliedOnContent = ContentReference.IsNullOrEmpty(value) ? string.Empty : value.ToReferenceWithoutVersion().ToString();
        }

        public string AppliedOnContent { get; set; }

        public string CurrentSettingsJson { get; set; }

        public string NewSettingsJson { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public DateTime Saved { get; set; }

        public bool CanExecute { get; set; }

        public string AppliedOnLanguageBranch { get; set; }

        public int Status { get; set; }

        public string TypeIdentifier { get; set; }

        public bool IsCurrentUserInActiveStepReviewerList { get; set; }

        public bool IsCommandDataValid { get; set; }

        public bool CanUserActOnHisOwnChanges { get; set; }
    }
}