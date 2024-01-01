namespace AdvancedTaskManager.Infrastructure.Cms.ChangeApproval
{
    public class ContentChangeDetails : IContentChangeDetails
    {
        public string Name { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}