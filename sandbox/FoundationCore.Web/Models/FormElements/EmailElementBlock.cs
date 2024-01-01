using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Web;



namespace FoundationCore.Web.Models.FormElements;

[ContentType(GUID = "3645255c-1b47-4a2d-9756-e9b4127286d1", DisplayName = "Email form elements", GroupName = "Custom form elements", Order = 3000)]
[AvailableValidatorTypes(Include = new[] { typeof(RequiredValidator) })]
public class EmailElementBlock : TextboxElementBlock
{
    [Display(GroupName = "Information")]
    [CultureSpecific]
    [UIHint(UIHint.Textarea)]
    public virtual string Email { get; set; }

}