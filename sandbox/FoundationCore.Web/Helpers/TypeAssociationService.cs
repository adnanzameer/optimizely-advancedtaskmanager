using System.Reflection;
using EPiServer.ServiceLocation;
using FoundationCore.Web.Models.Pages;

namespace FoundationCore.Web.Helpers
{
    [ServiceConfiguration(ServiceType = typeof(ITypeAssociationService))]
    public class TypeAssociationService : ITypeAssociationService
    {
        private IContentTypeRepository _contentTypeRepo;
        private IContentModelUsage _contentModelUsage;
        private IContentRepository _contentRepo;

        public TypeAssociationService(
              IContentTypeRepository contentTypeRepo
            , IContentModelUsage contentModelUsage
            , IContentRepository contentRepo)
        {
            _contentTypeRepo = contentTypeRepo;
            _contentModelUsage = contentModelUsage;
            _contentRepo = contentRepo;
        }

        public IEnumerable<ContentType> AllPageTypes()
        {
            return GetTypes<PageData>()
                .OrderBy(p => !string.IsNullOrEmpty(p.DisplayName) ? p.DisplayName : p.Name);
        }

        public IEnumerable<ContentType> AllCmsCreatableBlockTypes()
        {
            return GetTypes<BlockData>()
                .Where(x => x.ModelType.GetCustomAttribute<ContentTypeAttribute>(false)?.AvailableInEditMode == true)
                .OrderBy(p => !string.IsNullOrEmpty(p.DisplayName) ? p.DisplayName : p.Name);
        }

        public IEnumerable<string> GetTypePropNames(int typeId) =>
            _contentTypeRepo.Load(typeId)
                            .PropertyDefinitions?
                            .Where(prop => prop.Type.DataType == PropertyDataType.String
                                        || prop.Type.DataType == PropertyDataType.LongString)
                            .Select(p => p.Name)?
                            .OrderBy(name => name);

        public IEnumerable<IContent> GetTypeUsages(int TypeId)
        {
            var contentType = _contentTypeRepo.Load(TypeId);

            if (contentType == null)
                return null;

            return _contentModelUsage.ListContentOfContentType(contentType)
                                     .Select(c => c.ContentLink.ToReferenceWithoutVersion())
                                     .Distinct()
                                     .Select(c => _contentRepo.Get<IContent>(c));
        }

        public int GetContentUsageCount(ContentReference cr)
        {
            return _contentRepo.GetReferencesToContent(cr, false).Count();
        }

        public IEnumerable<ContentType> GetTypes<T>() where T : IContentData
        {
            return _contentTypeRepo.List().Where(c => typeof(T).IsAssignableFrom(c.ModelType));
        }

        public IEnumerable<T> GetContentUsagesOfSitePageData<T>() where T : SitePageData
        {
            var contentType = GetTypes<T>().FirstOrDefault();

            if (contentType == null) return null;

            return _contentModelUsage.ListContentOfContentType(contentType)
                .Select(c => c.ContentLink.ToReferenceWithoutVersion())
                .Distinct()
                .Select(c => _contentRepo.Get<IContent>(c) as T)
                .Where(c => c != null)
                .ToList();
        }

        public IEnumerable<T> GetContentUsagesOfBlockData<T>() where T : BlockData
        {
            var contentType = GetTypes<T>().FirstOrDefault();

            if (contentType == null) return null;

            return _contentModelUsage.ListContentOfContentType(contentType)
                .Select(c => c.ContentLink.ToReferenceWithoutVersion())
                .Distinct()
                .Select(c => _contentRepo.Get<IContent>(c) as T)
                .Where(c => c != null)
                .ToList();
        }


        public IEnumerable<T> GetContentUsagesOfImageData<T>() where T : ImageData
        {
            var contentType = GetTypes<T>().FirstOrDefault();

            if (contentType == null) return null;

            return _contentModelUsage.ListContentOfContentType(contentType)
                .Select(c => c.ContentLink.ToReferenceWithoutVersion())
                .Distinct()
                .Select(c => _contentRepo.Get<IContent>(c) as T)
                .Where(c => c != null)
                .ToList();
        }
    }
}
