using System;
using AdvancedTaskManager.Infrastructure.Cms.ChangeApproval;

namespace AdvancedTaskManager.Infrastructure.Helpers
{
    public interface IApprovalCommandService
    {
        ApprovalCommandBase GetCommandById(Guid commandId);
        ApprovalCommandBase GetCommandByApprovalId(int approvalId);
    }

    public class ApprovalCommandService : IApprovalCommandService
    {
        private readonly ICommandMetaDataRepository _commandMetaDataRepository;
        private readonly IApprovalCommandRepositoryBase _approvalCommandRepositoryBase;

        public ApprovalCommandService(ICommandMetaDataRepository cmdRepository, IApprovalCommandRepositoryBase approvalCommandRepositoryBase)
        {
            _commandMetaDataRepository = cmdRepository;
            _approvalCommandRepositoryBase = approvalCommandRepositoryBase;
        }

        public ApprovalCommandBase GetCommandById(Guid commandId)
        {
            var byCommandId = _commandMetaDataRepository.GetByCommandId(commandId);
            return byCommandId == null ? null : GetApprovalCommand(byCommandId.Type, byCommandId.CommandId);
        }


        public ApprovalCommandBase GetCommandByApprovalId(int approvalId)
        {
            var byApprovalId = _commandMetaDataRepository.GetByApprovalId(approvalId);
            return byApprovalId == null ? null : GetApprovalCommand(byApprovalId.Type, byApprovalId.CommandId);
        }


        public CommandMetaData GetCommandMetaDataByApprovalId(int approvalId)
        {
            var byApprovalId = _commandMetaDataRepository.GetByApprovalId(approvalId);
            return byApprovalId;
        }

        private ApprovalCommandBase GetApprovalCommand(string commandTypeName, Guid commandId)
        {
            ApprovalCommandBase byCommandId = null;

            if (commandTypeName.EndsWith("MovingContentCommand"))
            {
                byCommandId = _approvalCommandRepositoryBase.GetByCommandId<MovingContentCommand>(commandId, commandTypeName);
            }
            else if (commandTypeName.EndsWith("ExpirationDateSettingCommand"))
            {
                byCommandId = _approvalCommandRepositoryBase.GetByCommandId<ExpirationDateSettingCommand>(commandId, commandTypeName);
            }
            else if (commandTypeName.EndsWith("LanguageSettingCommand"))
            {
                byCommandId = _approvalCommandRepositoryBase.GetByCommandId<LanguageSettingCommand>(commandId, commandTypeName);
            }
            else if (commandTypeName.EndsWith("SecuritySettingCommand"))
            {
                byCommandId = _approvalCommandRepositoryBase.GetByCommandId<SecuritySettingCommand>(commandId, commandTypeName);
            }

            return byCommandId;
        }

    }
}
