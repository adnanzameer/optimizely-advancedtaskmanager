using System;
using System.Linq;
using AdvancedTaskManager.Infrastructure.Cms.ChangeApproval;
using EPiServer.Core;

namespace AdvancedTaskManager.Infrastructure.Helpers
{
    public static class ChangeApprovalReferenceHelper
    {
        public static Uri GetUri(ContentReference contentLink, bool ignoreVersion)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
                return null;
            var uriString =
                $"{ChangeApprovalTypeFactory.ChangeApprovalType}:{contentLink.ProviderName ?? ""}/{contentLink.ID}/";
            if (!ignoreVersion && contentLink.WorkID != 0)
                uriString = uriString + contentLink.WorkID + "/";
            return new Uri(uriString);
        }

        public static ContentReference GetContentReference(Uri key)
        {
            if (key == null || !key.Scheme.Equals(ChangeApprovalTypeFactory.ChangeApprovalType))
                return null;
            var list = key.Segments.Select(x => x.Replace("/", "")).ToList();
            if (list.Count < 2 || list.Count > 3)
                return null;
            var providerName = string.IsNullOrEmpty(list[0]) ? null : list[0];
            if (!int.TryParse(list[1], out var result1))
                return null;
            var result2 = 0;
            return list.Count > 2 && list[2] != string.Empty && !int.TryParse(list[2], out result2) ? null : new ContentReference(result1, result2, providerName);
        }
    }
}