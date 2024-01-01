using System;
using System.Collections.Generic;
using AdvancedTaskManager.Features.AdvancedTask;
using EPiServer.Logging;
using Newtonsoft.Json;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface IExpirationChangeDetails
    {
        IEnumerable<IContentChangeDetails> GetExpirationCommandChangeDetails(ChangeTaskViewModel model);
    }

    public class ExpirationChangeDetails : IExpirationChangeDetails
    {
        private readonly ILogger _logger;

        public ExpirationChangeDetails()
        {
            _logger = LogManager.GetLogger(typeof(ExpirationChangeDetails));
        }

        public IEnumerable<IContentChangeDetails> GetExpirationCommandChangeDetails(ChangeTaskViewModel model)
        {
            var interceptPropertyList = new List<string>() { "PageStopPublish", "PageArchiveLink" };
            var contentChangeDetailsList = new List<IContentChangeDetails>();

            try
            {
                var dictionary1 = JsonConvert.DeserializeObject<IDictionary<string, object>>(model.CurrentSettingsJson);
                var dictionary2 = JsonConvert.DeserializeObject<IDictionary<string, object>>(model.NewSettingsJson);
                foreach (var interceptProperty in interceptPropertyList)
                {
                    if (dictionary1.ContainsKey(interceptProperty))
                    {
                        var oldValue = dictionary1[interceptProperty];
                        var newValue = dictionary2[interceptProperty];

                        var item = new ContentChangeDetails
                        {
                            Name = GetExpirationDateSettingCommand(interceptProperty.ToLowerInvariant()),
                            OldValue = oldValue is DateTime oldValueDateTime
                                ? oldValueDateTime.ToString("MMM dd, yyyy, h:mm:ss tt")
                                : oldValue,
                            NewValue = newValue is DateTime newValueDateTime
                                ? newValueDateTime.ToString("MMM dd, yyyy, h:mm:ss tt")
                                : newValue
                        };

                        contentChangeDetailsList.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return contentChangeDetailsList;
        }

        static string GetExpirationDateSettingCommand(string interceptProperty)
        {
            switch (interceptProperty.ToLowerInvariant())
            {
                case "pagearchivelink":
                    return "Archive to";
                case "pagestoppublish":
                    return "Expiration date";
                default:
                    return "[Location is no longer available]";
            }
        }
    }
}
