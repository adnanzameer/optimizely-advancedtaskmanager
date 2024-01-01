using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Notification;
using EPiServer.Security;

namespace AdvancedTaskManager.Infrastructure.Helpers
{
    public interface IUIHelper
    {
        string GetDisplayNameForUser(string senderUsername);
        bool CanUserPublish<T>(T content) where T : IContent;
        Task<List<string>> GetUserRoles();
    }

    public class UIHelper : IUIHelper
    {
        private readonly QueryableNotificationUserService _queryableNotificationUserService;
        private readonly SecurityEntityProvider _securityEntityProvider;
        public UIHelper(QueryableNotificationUserService queryableNotificationUserService, SecurityEntityProvider securityEntityProvider)
        {
            _queryableNotificationUserService = queryableNotificationUserService;
            _securityEntityProvider = securityEntityProvider;
        }

        public string GetDisplayNameForUser(string senderUsername)
        {
            if (string.IsNullOrEmpty(senderUsername))
                return null;
            var result = _queryableNotificationUserService.GetAsync(senderUsername).ConfigureAwait(false).GetAwaiter().GetResult();

            if (result == null)
                return null;

            if (PrincipalAccessor.Current.Identity != null)
            {
                var name = PrincipalAccessor.Current.Identity.Name;
                if (result.UserName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return "You";
            }

            return !string.IsNullOrEmpty(result.DisplayName) ? result.DisplayName : result.UserName;
        }

        public async Task<List<string>> GetUserRoles()
        {
            if (PrincipalAccessor.Current.Identity != null)
            {
                var name = PrincipalAccessor.Current.Identity.Name;
                var roles = await _securityEntityProvider.GetRolesForUserAsync(name);
                return roles.ToList();
            }

            return new List<string>();
        }


        public bool CanUserPublish<T>(T content) where T : IContent
        {
            if (content is ISecurable securedContent)
            {
                var descriptor = securedContent.GetSecurityDescriptor();

                return descriptor.HasAccess(PrincipalAccessor.Current, AccessLevel.Publish);
            }
            return false;
        }
    }
}