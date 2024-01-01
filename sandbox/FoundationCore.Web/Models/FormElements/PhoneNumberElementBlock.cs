using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Web;



namespace FoundationCore.Web.Models.FormElements;

[ContentType(GUID = "0dd1a9e1-27d6-4777-8455-e12723ee2ef5", DisplayName = "Phone number form elements", GroupName = "Custom form elements", Order = 2000)]
[AvailableValidatorTypes(Include = new[] { typeof(RequiredValidator) })]
public class PhoneNumberElementBlock : TextboxElementBlock
{
    [Display(GroupName = "Information")]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string PhoneNumber { get; set; }

}