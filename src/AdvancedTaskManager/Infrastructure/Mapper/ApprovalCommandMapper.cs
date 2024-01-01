using System;
using System.Globalization;
using System.Security.Principal;
using AdvancedTaskManager.Features.AdvancedTask;
using AdvancedTaskManager.Infrastructure.Cms.ChangeApproval;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer;
using EPiServer.Core;

namespace AdvancedTaskManager.Infrastructure.Mapper
{
    public interface IApprovalCommandMapper
    {
        ChangeTaskViewModel Map(ApprovalCommandBase approvalCommand, IPrincipal principal);
    }

    public class ApprovalCommandMapper : IApprovalCommandMapper
    {
        private readonly ViewModelMapper _mapper = new();
        private readonly IContentRepository _contentRepository;
        private readonly IUIHelper _uiHelper;

        public ApprovalCommandMapper(IContentRepository contentRepository, IUIHelper uiHelper)
        {
            _contentRepository = contentRepository;
            _uiHelper = uiHelper;
            _mapper.Add<SecuritySettingCommand, ChangeTaskViewModel>();
            _mapper.Add<LanguageSettingCommand, ChangeTaskViewModel>();
            _mapper.Add<ExpirationDateSettingCommand, ChangeTaskViewModel>();
            _mapper.Add<MovingContentCommand, ChangeTaskViewModel>();
        }

        public ChangeTaskViewModel Map(ApprovalCommandBase approvalCommand, IPrincipal principal)
        {
            var commandViewModel1 = _mapper.Map(approvalCommand) as ChangeTaskViewModel;
            var name1 = approvalCommand is ICultureSpecificApprovalCommand specificApprovalCommand ? specificApprovalCommand.AppliedOnLanguageBranch : null;
            var cultureInfo = string.IsNullOrEmpty(name1) ? null : new CultureInfo(name1);
            var commandViewModel2 = commandViewModel1;
            var contentRepository = _contentRepository;
            var appliedOnContentLink = approvalCommand.AppliedOnContentLink;
            var settings = new LoaderOptions
            {
                new LanguageLoaderOption
                {
                    Language = cultureInfo, FallbackBehaviour = LanguageBehaviour.FallbackWithMaster
                }
            };

            var name2 = contentRepository.Get<IContent>(appliedOnContentLink, settings)?.Name;

            if (commandViewModel2 != null)
                commandViewModel2.Name = name2;

            if (commandViewModel1 != null)
            {
                commandViewModel1.Status = (int)approvalCommand.CommandStatus;
                commandViewModel1.CanExecute = approvalCommand.CommandStatus == CommandMetaData.ChangeTaskApprovalStatus.InReview;
                var fullName = approvalCommand.GetType().FullName;
                if (fullName != null)
                    commandViewModel1.TypeIdentifier = fullName.ToLower();

                commandViewModel1.Id = approvalCommand.Id.ExternalId.ToString();
                commandViewModel1.IsCommandDataValid = approvalCommand.IsValid();
                commandViewModel1.CreatedBy = _uiHelper.GetDisplayNameForUser(commandViewModel1.CreatedBy);
                commandViewModel1.ChangedBy = _uiHelper.GetDisplayNameForUser(commandViewModel1.ChangedBy);
                if (principal.Identity != null)
                    commandViewModel1.CanUserActOnHisOwnChanges = !string.Equals(approvalCommand.CreatedBy, principal.Identity.Name, StringComparison.OrdinalIgnoreCase);
                return commandViewModel1;
            }

            return null;
        }
    }
}
