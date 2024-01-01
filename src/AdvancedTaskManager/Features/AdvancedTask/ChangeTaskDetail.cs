using System.Collections.Generic;
using AdvancedTaskManager.Infrastructure.Cms.ChangeApproval;

namespace AdvancedTaskManager.Features.AdvancedTask
{
    public class ChangeTaskDetail
    {
        public ChangeTaskDetail()
        {
            Details = new List<IContentChangeDetails>();
        }

        public IEnumerable<IContentChangeDetails> Details { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

    }
}
