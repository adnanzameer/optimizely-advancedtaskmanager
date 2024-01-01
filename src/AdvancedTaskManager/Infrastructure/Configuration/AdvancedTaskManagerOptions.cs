namespace AdvancedTaskManager.Infrastructure.Configuration
{
    public class AdvancedTaskManagerOptions
    {
        public int WarningDays { get; set; } = 4;
        public int PageSize { get; set; } = 30;
        public bool DeleteChangeApprovalTasks { get; set; } = true;
        public bool DeleteContentApprovalDeadlineProperty { get; set; } = false;
        public bool AddContentApprovalDeadlineProperty { get; set; } = false;
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm";
        public string DateTimeFormatUserFriendly { get; set; } = "MMM dd, yyyy, h:mm:ss tt";
    }
}