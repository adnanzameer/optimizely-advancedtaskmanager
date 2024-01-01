using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace FoundationCore.Web.Business.EditorDescriptors;

[EditorDescriptorRegistration(TargetType = typeof(string[]), UIHint = Globals.SiteUIHints.Strings)]
public class StringListEditorDescriptor : EditorDescriptor
{
    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        ClientEditingClass = "foundationcore/editors/StringList";

        base.ModifyMetadata(metadata, attributes);
    }
}