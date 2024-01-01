using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using FoundationCore.Web.Helpers;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class FileBasedEventsInitialization : IInitializableModule
    {
        // attach to both the CreatingContent and SavingContent events
        public void Initialize(InitializationEngine context)
        {
            var eventRegistry = ServiceLocator.Current.GetInstance<IContentEvents>();

            eventRegistry.CreatingContent += SavingMedia;
            eventRegistry.SavingContent += SavingMedia;
        }

        private void SavingMedia(object sender, ContentEventArgs e)
        {

            if (!(e.Content is IContentMedia))
                return;

            var media = e.Content as IContentMedia;

            var fileSize = GetFileSizeDisplay(media);

            if (media is IHasFileSize file)
            {
                file.FileSize = fileSize;
            }
        }

        private string GetFileSizeDisplay(IContentMedia media)
        {
            if (media?.BinaryData != null)
            {
                using var stream = media.BinaryData.OpenRead();
                return PageHelper.FormatBytes(stream.Length);
            }

            return string.Empty;
        }

        public void Uninitialize(InitializationEngine context)
        {
            var eventRegistry = ServiceLocator.Current.GetInstance<IContentEvents>();

            eventRegistry.CreatingContent -= SavingMedia;
            eventRegistry.SavingContent -= SavingMedia;
        }

    }
}
