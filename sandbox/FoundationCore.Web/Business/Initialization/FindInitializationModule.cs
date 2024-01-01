using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Conventions;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using FoundationCore.Web.Models.Pages;

namespace FoundationCore.Web.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class FindInitializationModule : IInitializableModule
    {
        private ContentAssetHelper _contentAssetHelper;
        private bool _initialized;
        public void Initialize(InitializationEngine context)
        {
            if (_initialized)
                return;

            _contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();

            ContentIndexer.Instance.Conventions.ForInstancesOf<ContentFolder>().ShouldIndex(x => false);
            ContentIndexer.Instance.Conventions.ForInstancesOf<ContentAssetFolder>().ShouldIndex(x => false);

            //Media
            ContentIndexer.Instance.Conventions.ForInstancesOf<MediaData>().ShouldIndex(p => ShouldIndexDocument(p));

            ContentIndexer.Instance.Conventions.ForInstancesOf<SitePageData>()
                .ShouldIndex(x =>
                    x.StartPublish != null
                    && !x.IsDeleted
                    && !x.DisableIndexing
                    && x.ContentLink.ID != ContentReference.WasteBasket.ID
                    && !x.Ancestors().Contains(ContentReference.WasteBasket.ID.ToString())
                    && (!x.StopPublish.HasValue || x.StopPublish.Value >= DateTime.Now));

            _initialized = true;
        }

        bool ShouldIndexDocument(MediaData content)
        {
            if (_contentAssetHelper.GetAssetOwner(content.ContentLink) is FileUploadElementBlock)
            {
                //if descendant of episerver forms or a file uplaoded through a epi form, do not index
                return false;
            }
            return !content.IsDeleted && IsNotArchived(content.StopPublish);
        }

        static bool IsNotArchived(DateTime? stopPublishDate)
        {
            return stopPublishDate == null || stopPublishDate > DateTime.Now;
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Required by InitializableModule
        }
    }
}