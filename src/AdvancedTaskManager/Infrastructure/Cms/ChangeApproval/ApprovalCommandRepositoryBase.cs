using System;
using System.Linq;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface IApprovalCommandRepositoryBase
    {
        ApprovalCommandBase GetByCommandId<T>(Guid id, string type) where T : ApprovalCommandBase;
    }

    public class ApprovalCommandRepositoryBase : IApprovalCommandRepositoryBase
    {
        private readonly ChangeApprovalDynamicDataStoreFactory _changeApprovalDynamicDataStoreFactory;

        public ApprovalCommandRepositoryBase(ChangeApprovalDynamicDataStoreFactory changeApprovalDynamicDataStoreFactory)
        {
            _changeApprovalDynamicDataStoreFactory = changeApprovalDynamicDataStoreFactory;
        }

        public ApprovalCommandBase GetByCommandId<T>(Guid id, string type) where T : ApprovalCommandBase
        {
            var store = _changeApprovalDynamicDataStoreFactory.GetStore(type);

            var obj = store?.Items<T>().FirstOrDefault(x => x.Id.ExternalId == id);

            return obj;
        }
    }
}