using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace FoundationCore.Web.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                return "";
            }

            var sb = new StringBuilder();
            for (var i = 0; i < collection.Count; i++)
            {
                var key = collection.Keys[i];
                var values = collection.GetValues(key);
                if (values == null)
                {
                    continue;
                }

                foreach (var value in values)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        sb.Append(key).Append('=');
                    }
                    sb.Append(WebUtility.UrlEncode(value)).Append('&');
                }
            }

            return sb.ToString(0, sb.Length - 1);
        }
    }
}
