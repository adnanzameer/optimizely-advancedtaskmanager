using FoundationCore.Web.Models.Pages;

namespace FoundationCore.Web.Helpers
{
    public interface ITypeAssociationService
    {
        IEnumerable<ContentType> AllPageTypes();
        IEnumerable<ContentType> AllCmsCreatableBlockTypes();
        IEnumerable<string> GetTypePropNames(int typeId);
        IEnumerable<IContent> GetTypeUsages(int TypeId);
        int GetContentUsageCount(ContentReference cr);
        IEnumerable<ContentType> GetTypes<T>() where T : IContentData;
        IEnumerable<T> GetContentUsagesOfSitePageData<T>() where T : SitePageData;
        IEnumerable<T> GetContentUsagesOfBlockData<T>() where T : BlockData;
        IEnumerable<T> GetContentUsagesOfImageData<T>() where T : ImageData;

    }
}