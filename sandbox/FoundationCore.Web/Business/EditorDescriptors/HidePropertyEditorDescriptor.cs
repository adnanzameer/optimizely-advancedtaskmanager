using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace FoundationCore.Web.Business.EditorDescriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentData))]
    public class HidePropertyEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            foreach (var property in metadata.Properties)
            {
                if (property.PropertyName == "PageVisibleInMenu")
                {
                    property.ShowForEdit = false;
                }
            }
        }
    }
}
