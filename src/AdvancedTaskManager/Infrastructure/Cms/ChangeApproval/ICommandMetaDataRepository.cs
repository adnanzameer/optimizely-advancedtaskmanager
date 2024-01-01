using System;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface ICommandMetaDataRepository
    {
        CommandMetaData GetByApprovalId(int approvalId);
        CommandMetaData GetByCommandId(Guid commandId);
        Guid Save(CommandMetaData commandMetaData);
    }
}

