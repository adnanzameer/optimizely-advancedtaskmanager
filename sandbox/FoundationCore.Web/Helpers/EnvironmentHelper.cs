namespace FoundationCore.Web.Helpers
{
    public static class EnvironmentHelper
    {
        public static bool IsLocal()
        {
            var environmentName = GetEnvironmentName();
            return string.IsNullOrEmpty(environmentName) || environmentName.Equals("Local") || environmentName.Equals("Development");
        }
        public static bool IsIntegration()
        {
            var environmentName = GetEnvironmentName();
            return !string.IsNullOrEmpty(environmentName) && environmentName.Equals("Integration");
        }
        public static bool IsPreproduction()
        {
            var environmentName = GetEnvironmentName();
            return !string.IsNullOrEmpty(environmentName) && environmentName.Equals("Preproduction");
        }

        public static bool IsProduction()
        {
            var environmentName = GetEnvironmentName();
            return !string.IsNullOrEmpty(environmentName) && environmentName.Equals("Production");
        }

        public static string GetEnvironmentName() { return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); }
    }
}
