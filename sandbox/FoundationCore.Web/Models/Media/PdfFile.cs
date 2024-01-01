using System.ComponentModel.DataAnnotations;
using EPiServer.Find;
using EPiServer.Framework.DataAnnotations;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Models.Media
{
    [ContentType(DisplayName = "PdfFile", GUID = "285622fe-0e42-4cc5-ad9a-f04657c9ff8d")]
    [MediaDescriptor(ExtensionString = "pdf")]
    public class PdfFile : MediaData, ISearchableContent, IHasFileSize
    {
        [Editable(false)]
        [Display(Order = 10, GroupName = SystemTabNames.Settings, Name = "File Size")]
        public virtual string FileSize { get; set; }

        public IDictionary<string, IndexValue> SearchMetaData
        {
            get
            {
                var dictionary = new Dictionary<string, IndexValue>
                {
                    {"size", FileSize}
                };

                return dictionary;
            }
        }
    }
}
