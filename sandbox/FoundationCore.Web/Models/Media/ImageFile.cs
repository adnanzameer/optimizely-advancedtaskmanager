using EPiServer.Framework.DataAnnotations;

namespace FoundationCore.Web.Models.Media;

[ContentType(GUID = "0A89E464-56D4-449F-AEA8-2BF774AB8730")]
[MediaDescriptor(ExtensionString = "jpg,jpeg,jpe,ico,gif,bmp,png")]
public class ImageFile : ImageData
{
    public virtual string AltText { get; set; }
    public virtual string Copyright { get; set; }
}
