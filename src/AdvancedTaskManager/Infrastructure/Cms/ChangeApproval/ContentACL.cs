using System.Collections.Generic;
using EPiServer.Security;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public class ContentACL
    {
        public IEnumerable<AccessControlEntry> Entries { get; set; }

        public bool IsInherited { get; set; }
    }
}