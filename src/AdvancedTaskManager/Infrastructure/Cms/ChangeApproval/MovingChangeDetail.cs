using System;
using System.Collections.Generic;
using AdvancedTaskManager.Features.AdvancedTask;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using Newtonsoft.Json;

namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface IMovingChangeDetail
    {
        IEnumerable<IContentChangeDetails> GetMovingChangeDetails(ChangeTaskViewModel byCommandId);
    }

    public class MovingChangeDetail : IMovingChangeDetail
    {
        private readonly ILogger _logger;

        private readonly IContentLoader _contentLoader;

        public MovingChangeDetail(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
            _logger = LogManager.GetLogger(typeof(MovingChangeDetail));
        }


        #region moving

        public IEnumerable<IContentChangeDetails> GetMovingChangeDetails(ChangeTaskViewModel byCommandId)
        {

            if (byCommandId == null)
                return null;
            var contentChangeDetailsList = new List<IContentChangeDetails>();
            try
            {
                var movingPayLoad1 = JsonConvert.DeserializeObject<MovingPayLoad>(byCommandId.CurrentSettingsJson);
                var movingPayLoad2 = JsonConvert.DeserializeObject<MovingPayLoad>(byCommandId.NewSettingsJson);

                var content1 = GetContentPathString(movingPayLoad1.Destination);
                var content2 = GetContentPathString(movingPayLoad2.Destination);
                contentChangeDetailsList.Add(new ContentChangeDetails()
                {
                    Name = "Path",
                    OldValue = content1,
                    NewValue = content2
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return contentChangeDetailsList;
        }

        private string GetContentPathString(ContentReference contentReference)
        {
            // Retrieve the content
            var content = _contentLoader.Get<IContent>(contentReference);

            // Get the content path including all its parents
            var contentPath = GetContentPath(content);

            return contentPath;
        }

        private string GetContentPath(IContent content)
        {
            // Use the ContentLoader to get the content path
            var parentReference = content.ParentLink;

            var path = content.Name; // Start with the current content's name

            while (parentReference.ID != ContentReference.RootPage.ID)
            {
                var parentContent = _contentLoader.Get<IContent>(parentReference);

                path = $"{parentContent.Name} > {path}";

                // Move up to the parent
                parentReference = parentContent.ParentLink;
            }

            return path + " >";
        }

        #endregion
    }
}
