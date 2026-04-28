using EPiServer.Data.Dynamic;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public class ChangeApprovalDynamicDataStoreFactory
    {
        public DynamicDataStore? GetStore(string name)
        {
            return DynamicDataStoreFactory.Instance.GetStore(name);
        }
    }
}
