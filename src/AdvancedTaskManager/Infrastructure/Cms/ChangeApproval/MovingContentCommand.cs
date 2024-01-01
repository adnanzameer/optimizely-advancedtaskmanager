using System;
using AdvancedTaskManager.Features.AdvancedTask;
using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Dynamic;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    [EPiServerDataStore(AutomaticallyRemapStore = true)]
    public class MovingContentCommand : ApprovalCommandBase
    {
        private static readonly ILogger Logger = LogManager.GetLogger(typeof(MovingContentCommand));
        private Injected<IContentLoader> _contentLoader;

        public override bool IsValid()
        {
            try
            {
                return _contentLoader.Service.Get<IContent>(JsonConvert.DeserializeObject<MovingPayLoad>(NewSettingsJson).Destination) != null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return false;
            }
        }
    }
}