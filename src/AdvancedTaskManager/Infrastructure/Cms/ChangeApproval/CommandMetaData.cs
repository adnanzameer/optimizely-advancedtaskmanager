using System;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class CommandMetaData : IDynamicData
    {
        private bool _isReadOnly;

        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public int ApprovalId { get; set; }

        public string Type { get; set; }

        [EPiServerDataIndex]
        public Guid CommandId { get; set; }

        [EPiServerDataIndex]
        public ContentReference AppliedOnContent { get; set; }

        public ChangeTaskApprovalStatus CommandStatus { get; set; }


        public enum ChangeTaskApprovalStatus
        {
            InReview,
            Approved
        }

        public bool IsReadOnly
        {
            get => _isReadOnly;
            protected set => _isReadOnly = value;
        }

        public void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        public object CreateWritableClone()
        {
            var commandMetaData = (CommandMetaData)MemberwiseClone();
            commandMetaData._isReadOnly = false;
            return commandMetaData;
        }
    }
}