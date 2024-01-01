using System;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer.Approvals;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public class ChangeApprovalTypeFactory : IApprovalTypeFactory
    {
        public static readonly string ChangeApprovalType = "changeapproval";
        private readonly IApprovalDefinitionReferenceResolver _approvalReferenceResolver;

        public ChangeApprovalTypeFactory(IApprovalDefinitionReferenceResolver approvalReferenceResolver)
        {
            _approvalReferenceResolver = approvalReferenceResolver;
        }

        public string ApprovalType => ChangeApprovalType;

        public IApprovalDefinitionReferenceResolver DefinitionReferenceResolver => _approvalReferenceResolver;

        public IApprovalLanguageResolver LanguageResolver => null;

        public Approval CreateApproval(Uri reference)
        {
            return new ChangeApproval
            {
                ContentLink = ChangeApprovalReferenceHelper.GetContentReference(reference)
            };
        }

        public ApprovalDefinition CreateApprovalDefinition(Uri reference)
        {
            throw new NotSupportedException("Change Approval does not support creating an approval definition.");
        }
    }
}