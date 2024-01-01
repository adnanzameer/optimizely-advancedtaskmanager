using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoundationCore.Web.Helpers
{
    public static class IHtmlHelperExtensions
    {
        internal static bool? GetFlagValueFromViewData(this IHtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary(key);
        }

        internal static string GetValueFromViewData(this IHtmlHelper htmlHelper, string key)
        {
            return htmlHelper.ViewContext.ViewData.GetValueFromDictionary<string>(key);
        }

        private static T GetValueFromDictionary<T>(this IDictionary<string, object> source, string key)
        {
            var actualValue = source[key];
            return (T)actualValue;
        }

        private static bool? GetValueFromDictionary(this IDictionary<string, object> source, string key)
        {
            var actualValue = source[key];
            bool? result = null;

            if (actualValue is bool value)
            {
                result = value;
            }

            return result;
        }
    }
}
