using EPiServer.Data.Dynamic;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class LanguageSettingCommand : ApprovalCommandBase
    {
    }
}
