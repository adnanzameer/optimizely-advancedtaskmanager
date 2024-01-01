using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AdvancedTaskManager.Infrastructure.Helpers;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.Shell.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface ISecurityChangeDetail
    {
        IEnumerable<IContentChangeDetails> GetSecurityChangeDetails(Guid command);
    }

    public class SecurityChangeDetail : ISecurityChangeDetail
    {
        private readonly IApprovalCommandService _generalCommandService;
        private readonly ILogger _logger;

        private static readonly List<AccessLevel> KnownAccessLevels = new()
        {
            AccessLevel.Read,
            AccessLevel.Create,
            AccessLevel.Edit,
            AccessLevel.Delete,
            AccessLevel.Publish,
            AccessLevel.Administer
        };

        public SecurityChangeDetail(IApprovalCommandService generalCommandService)
        {
            _generalCommandService = generalCommandService;
            _logger = LogManager.GetLogger(typeof(SecurityChangeDetail));
        }


        #region security

        public IEnumerable<IContentChangeDetails> GetSecurityChangeDetails(Guid command)
        {
            var byCommandId = _generalCommandService.GetCommandById(command);
            var contentChangeDetailsList = new List<IContentChangeDetails>();
            var str1 = "Yes";
            var str2 = "No";
            if (byCommandId == null)
                return null;
            var accessControlList1 = GetContentAccessControlList(byCommandId.CurrentSettingsJson);
            var accessControlList2 = GetContentAccessControlList(byCommandId.NewSettingsJson);
            if (accessControlList1 == null || accessControlList2 == null)
                return null;
            if (accessControlList1.IsInherited != accessControlList2.IsInherited)
            {
                var contentChangeDetails = (IContentChangeDetails)new ContentChangeDetails();
                contentChangeDetails.Name = "Inherit settings";
                contentChangeDetails.OldValue = accessControlList1.IsInherited ? str1 : str2;
                contentChangeDetails.NewValue = accessControlList2.IsInherited ? str1 : str2;
                contentChangeDetailsList.Add(contentChangeDetails);
            }

            if (byCommandId is SecuritySettingCommand secCommand && (secCommand.SecuritySaveType == SecuritySaveType.MergeChildPermissions || secCommand.SecuritySaveType == SecuritySaveType.ReplaceChildPermissions))
            {
                var contentChangeDetails = (IContentChangeDetails)new ContentChangeDetails();
                contentChangeDetails.Name = "Applied to all sub-items";
                contentChangeDetails.OldValue = string.Empty;
                contentChangeDetails.NewValue = str1;
                contentChangeDetailsList.Add(contentChangeDetails);
            }
            var contentChangeDetails1 = (IContentChangeDetails)new ContentChangeDetails();
            contentChangeDetails1.NewValue = "";
            contentChangeDetails1.OldValue = "";

            contentChangeDetails1.Name = "Access Control List";
            var source1 = accessControlList1.Entries.OrderBy(e => e.Name).ToList();
            var source2 = accessControlList2.Entries.OrderBy(e => e.Name).ToList();

            foreach (var accessControlEntry1 in source1)
            {
                var currentAccessControlItem = accessControlEntry1;
                var accessControlEntry2 = source2.FirstOrDefault(e => e.Name.Equals(currentAccessControlItem.Name, StringComparison.OrdinalIgnoreCase)) ?? new AccessControlEntry(currentAccessControlItem.Name, AccessLevel.Undefined);
                contentChangeDetails1.OldValue = contentChangeDetails1.OldValue + LocalizeCurrentAccessLevel(WebUtility.HtmlEncode(currentAccessControlItem.Name), currentAccessControlItem.Access, accessControlEntry2.Access) + "</br>";
            }

            foreach (var accessControlEntry1 in source2)
            {
                var newAccessControlItem = accessControlEntry1;
                var accessControlEntry2 = source1.FirstOrDefault(e => e.Name.Equals(newAccessControlItem.Name, StringComparison.OrdinalIgnoreCase)) ?? new AccessControlEntry(newAccessControlItem.Name, AccessLevel.Undefined);
                contentChangeDetails1.NewValue = contentChangeDetails1.NewValue + LocalizeNewAccessLevel(WebUtility.HtmlEncode(newAccessControlItem.Name), accessControlEntry2.Access, newAccessControlItem.Access) + "</br>";
            }
            contentChangeDetails1.OldValue = contentChangeDetails1.OldValue == null ? string.Empty : contentChangeDetails1.OldValue.ToString().TrimEnd("</br>");
            contentChangeDetails1.NewValue = contentChangeDetails1.NewValue == null ? string.Empty : contentChangeDetails1.NewValue.ToString().TrimEnd("</br>");
            contentChangeDetailsList.Add(contentChangeDetails1);
            return contentChangeDetailsList;
        }

        private string LocalizeCurrentAccessLevel(string accessLevelName, AccessLevel currentAccessLevel, AccessLevel newAccessLevel)
        {
            var str = $"{accessLevelName}: ";
            string text;
            if (currentAccessLevel == AccessLevel.NoAccess || currentAccessLevel == AccessLevel.Undefined)
            {
                text = GetAccessLevelDescription(currentAccessLevel);
            }
            else
            {
                foreach (var knownAccessLevel in KnownAccessLevels)
                {
                    if (currentAccessLevel.HasFlag(knownAccessLevel))
                        str = str + GetAccessLevelDescription(knownAccessLevel) + ", ";
                }
                text = str.TrimEnd(", ");
            }
            if (currentAccessLevel == newAccessLevel)
                text = text.Fade();
            return text;
        }

        private static string LocalizeNewAccessLevel(string accessLevelName, AccessLevel currentAccessLevel, AccessLevel newAccessLevel)
        {
            var str = $"{accessLevelName}: ";
            string text1;
            if ((newAccessLevel == AccessLevel.NoAccess || newAccessLevel == AccessLevel.Undefined) && newAccessLevel == currentAccessLevel)
            {
                text1 = GetAccessLevelDescription(newAccessLevel);
            }
            else
            {
                foreach (var knownAccessLevel in KnownAccessLevels)
                {
                    if (currentAccessLevel.HasFlag(knownAccessLevel) || newAccessLevel.HasFlag(knownAccessLevel))
                    {
                        var text2 = GetAccessLevelDescription(knownAccessLevel);
                        if (currentAccessLevel.HasFlag(knownAccessLevel) && !newAccessLevel.HasFlag(knownAccessLevel))
                        {
                            text2 = text2.Strikethrough();
                        }
                        else if (!currentAccessLevel.HasFlag(knownAccessLevel) && newAccessLevel.HasFlag(knownAccessLevel))
                        {
                            text2 = text2.Bold();
                        }

                        str = str + text2 + ", ";
                    }
                }
                text1 = str.TrimEnd(", ");
            }
            if (currentAccessLevel == newAccessLevel)
                text1 = text1.Fade();
            return text1;
        }
        private ContentACL GetContentAccessControlList(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            var jObject = JsonConvert.DeserializeObject<JObject>(json);

            var isInherited = jObject["IsInherited"]?.ToString() ?? "";

            return new ContentACL
            {
                IsInherited = isInherited.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase),
                Entries = GetAccessControlListFromJsonArray(jObject["Entries"])
            };
        }
        private List<AccessControlEntry> GetAccessControlListFromJsonArray(JToken entriesArray)
        {
            var accessControlEntryList = new List<AccessControlEntry>();
            try
            {
                if (entriesArray != null)
                    foreach (var entries in entriesArray)
                    {
                        var name = entries["Name"]?.ToString() ?? "";
                        int.TryParse(entries["Access"]?.ToString(), out var access);
                        int.TryParse(entries["EntityType"]?.ToString(), out var entry);

                        var accessControlEntry =
                            new AccessControlEntry(name, (AccessLevel)access, (SecurityEntityType)entry);
                        accessControlEntryList.Add(accessControlEntry);
                    }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return accessControlEntryList;
        }

        static string GetAccessLevelDescription(AccessLevel accessLevel)
        {
            switch (accessLevel)
            {
                case AccessLevel.NoAccess:
                    return "No Access";
                case AccessLevel.Read:
                    return "Read";
                case AccessLevel.Create:
                    return "Create";
                case AccessLevel.Edit:
                    return "Change";
                case AccessLevel.Delete:
                    return "Delete";
                case AccessLevel.Publish:
                    return "Publish";
                case AccessLevel.Administer:
                    return "Administer";
                case AccessLevel.FullAccess:
                    return "Full Access";
                case AccessLevel.Undefined:
                    return "Undefined";
                default:
                    return "Undefined";
            }
        }

        #endregion
    }
}
