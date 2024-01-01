using System;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.DataAnnotations;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public abstract class ApprovalCommandBase : IDynamicData
    {
        private bool _isReadOnly;

        public virtual Identity Id { get; set; }

        public int ApprovalID { get; set; }

        public int LastApprovalStepIndex { get; set; }

        [Ignore]
        public ContentReference AppliedOnContentLink
        {
            get => ContentReference.TryParse(AppliedOnContent, out var result) ? result : ContentReference.EmptyReference;
            set => AppliedOnContent = ContentReference.IsNullOrEmpty(value) ? string.Empty : value.ToReferenceWithoutVersion().ToString();
        }

        public string AppliedOnContent { get; set; }

        public string CurrentSettingsJson { get; set; }

        public string NewSettingsJson { get; set; }

        public CommandMetaData.ChangeTaskApprovalStatus CommandStatus { get; set; }

        public string CreatedBy { get; set; }

        public string ChangedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime Saved { get; set; }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            protected set => _isReadOnly = value;
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public object CreateWritableClone()
        {
            var approvalCommandBase = (ApprovalCommandBase)MemberwiseClone();
            approvalCommandBase._isReadOnly = false;
            return approvalCommandBase;
        }

    }
}
