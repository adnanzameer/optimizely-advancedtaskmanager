using System.ComponentModel.DataAnnotations;
using EPiServer.Find;
using EPiServer.Framework.DataAnnotations;
using FoundationCore.Web.Models.Interface;

namespace FoundationCore.Web.Models.Media
{
    [ContentType(DisplayName = "Document Media", GUID = "7a0e24b5-ce7f-4b1f-91e1-b4f21fb4fd61")]
    [MediaDescriptor(ExtensionString = "docx,xlsx")]
    public class DocumentMediaType : MediaData, IHasFileSize
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