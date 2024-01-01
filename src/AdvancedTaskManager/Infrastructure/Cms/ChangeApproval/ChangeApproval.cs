using System;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer.Approvals;
using EPiServer.Core;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public class ChangeApproval : Approval
    {
        private ContentReference _contentLink;

        public override Uri Reference => ChangeApprovalReferenceHelper.GetUri(_contentLink, false);

        public ContentReference ContentLink
        {
            get => _contentLink;
            set
            {
                ThrowIfReadOnly();
                _contentLink = value;
            }
        }

        public override Approval CreateWritableClone()
        {
            var writableClone = base.CreateWritableClone() as ChangeApproval;
            var contentLink = _contentLink;
            // ISSUE: explicit non-virtual call
            if (writableClone != null)
            {
                writableClone._contentLink = contentLink != null
                    ? contentLink.CreateWritableClone()
                    : null;
                return writableClone;
            }

            return null;
        }
    }
}