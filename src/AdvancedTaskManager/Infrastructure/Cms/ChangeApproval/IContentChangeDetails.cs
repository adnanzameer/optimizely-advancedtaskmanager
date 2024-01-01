namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public interface IContentChangeDetails
    {
        string Name { get; set; }

        object OldValue { get; set; }

        object NewValue { get; set; }
    }
}