namespace FoundationCore.Web.Models.Settings
{
    public abstract class AppSettingsBase<T> where T : AppSettingsBase<T>, new()
    {
        #region Properties

        protected abstract string ConfigurationSectionName { get; }

        #endregion

        public static string GetConfigurationSectionName() => new T().ConfigurationSectionName;

        public static IConfigurationSection GetSection(IConfiguration configuration)
            => configuration.GetSection(GetConfigurationSectionName());

        public static T Load(IConfiguration configuration)
            => configuration.GetSection(GetConfigurationSectionName()).Get<T>();
    }
}
