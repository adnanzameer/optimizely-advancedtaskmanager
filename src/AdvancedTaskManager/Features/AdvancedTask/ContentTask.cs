using System;
using System.Collections.Generic;
using AdvancedTaskManager.Infrastructure.Cms.ChangeApproval;
using EPiServer.Core;

namespace AdvancedTaskManager.Features.AdvancedTask
{
    public class ContentTask
    {
        public ContentTask()
        {
            ContentReference = ContentReference.EmptyReference;
            ContentName = "";
            ContentType = "";
            Type = "";
            DateTime = null;
            Deadline = null;
            StartedBy = "";
            WarningColor = "";
            Details = new List<IContentChangeDetails>();
            URL = "";
        }

        public int ApprovalId { get; set; }
        public ContentReference ContentReference { get; set; }
        public string ContentName { get; set; }
        public string ContentIcon { get; set; }
        public string ContentType { get; set; }
        public string Type { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? Deadline { get; set; }
        public string StartedBy { get; set; }
        public string WarningColor { get; set; }
        public bool NotificationUnread { get; set; }
        public bool CanUserPublish { get; set; }
        public string URL { get; set; }
        public IEnumerable<IContentChangeDetails> Details { get; set; }
    }
}

