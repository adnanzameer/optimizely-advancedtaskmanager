using System.ComponentModel.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.EditView.DataAnnotations;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Forms.Implementation.Validation;
using EPiServer.Web;



namespace FoundationCore.Web.Models.FormElements
{
    [ContentType(GUID = "be596250-fda6-43ae-af5b-ff17cb647f7a", GroupName = "Custom form elements", Order = 1000)]
    [AvailableValidatorTypes(Include = new[] { typeof(RequiredValidator) })]
    public class ConsentElementBlock : ValidatableElementBlockBase, IExcludeInSubmission
    {
        [Display(
            GroupName = "Information",
            Order = -8089)]
        [CultureSpecific]
        [UIHint(UIHint.Textarea)]
        public virtual XhtmlString ConsentText { get; set; }

        [ScaffoldColumn(false)] public override string Label { get; set; }

        [ScaffoldColumn(false)] public override string Description { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            // Seems to be the way to get "Required" by default
            // when adding the element to a form container
            Validators = typeof(RequiredValidator).FullName;

            Content.Name = "Consent checkbox";
        }
    }
}