using System;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.ServiceLocation;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    [ServiceConfiguration(ServiceType = typeof(ICommandMetaDataRepository))]
    public class DefaultCommandMetaDataRepository : ICommandMetaDataRepository
    {
        private static readonly object Lock = new();
        private readonly ChangeApprovalDynamicDataStoreFactory _changeApprovalDynamicDataStoreFactory;

        public DefaultCommandMetaDataRepository(
          ChangeApprovalDynamicDataStoreFactory changeApprovalDynamicDataStoreFactory)
        {
            _changeApprovalDynamicDataStoreFactory = changeApprovalDynamicDataStoreFactory;
        }

        public CommandMetaData GetByApprovalId(int approvalId)
        {
            var store = _changeApprovalDynamicDataStoreFactory.GetStore("EPiServer.ChangeApproval.Core.Internal.CommandMetaData");

            if (store == null)
                return null;
            lock (Lock)
            {
                return store.Items<CommandMetaData>().SingleOrDefault((Expression<Func<CommandMetaData, bool>>)(command => command.ApprovalId == approvalId));
            }
        }

        public CommandMetaData GetByCommandId(Guid commandId)
        {
            var store = _changeApprovalDynamicDataStoreFactory.GetStore("EPiServer.ChangeApproval.Core.Internal.CommandMetaData");
            if (store == null)
                return null;
            lock (Lock)
            {
                var commandMetaData = store.Items<CommandMetaData>().SingleOrDefault((Expression<Func<CommandMetaData, bool>>)(command => command.CommandId == commandId));
                return commandMetaData;
            }
        }


        public Guid Save(CommandMetaData commandMetaData)
        {
            var store = _changeApprovalDynamicDataStoreFactory.GetStore("EPiServer.ChangeApproval.Core.Internal.CommandMetaData");
            if (store == null)
                return Guid.Empty;
            lock (Lock)
            {
                var identity = store.Save(commandMetaData);
                return identity == null ? Guid.Empty : identity.ExternalId;
            }
        }
    }
}
