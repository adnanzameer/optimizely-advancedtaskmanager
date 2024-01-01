namespace AdvancedTaskManager.Infrastructure.Helpers
{
    public static class SettingCommandHelper
    {
        public static string Fade(this string text)
        {
            return !string.IsNullOrEmpty(text) ? $"<span class='epi-changeapproval-faded'>{text}</span>" : "";
        }

        public static string Bold(this string text)
        {
            return !string.IsNullOrEmpty(text) ? $"<strong>{text}</strong>" : "";
        }

        public static string Strikethrough(this string text)
        {
            return !string.IsNullOrEmpty(text) ? $"<del>{text}</del>" : "";
        }
    }
}