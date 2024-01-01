using System.Collections.Specialized;
using System.Net;

namespace FoundationCore.Web.Extensions
{

    public static class UriExtensions
    {
        public static string ResetFilters(this Uri link)
        {
            var newQueryString = link.Query.ParseQueryString();
            newQueryString.Remove("cf");
            newQueryString.Remove("filter");
            newQueryString.Remove("u");
            newQueryString.Remove("lang");
            newQueryString.Remove("m");

            var pagePathWithoutQueryString = link.GetLeftPart(UriPartial.Path);
            return newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{newQueryString}"
                : pagePathWithoutQueryString;
        }

        public static string AddQueryString(Uri link, string queryString, string queryStringValue = "",
            bool append = false)
        {
            var newQueryString = link.Query.ParseQueryString();
            newQueryString.Remove("id");
            newQueryString.Remove("page");
            newQueryString.Remove("epslanguage");
            newQueryString.Remove("p");

            if (!string.IsNullOrEmpty(queryString))
            {
                if (!append)
                {
                    newQueryString.Remove(queryString);
                    newQueryString.Add(queryString, queryStringValue);
                }
                else
                {
                    var values = newQueryString.Get(queryString);
                    if (!string.IsNullOrEmpty(values))
                    {
                        var valueList = values.Split(',').ToList();
                        if (!valueList.Contains(queryStringValue))
                            valueList.Add(queryStringValue);

                        newQueryString.Set(queryString, string.Join(",", valueList));
                    }
                    else
                    {
                        newQueryString.Add(queryString, queryStringValue);
                    }
                }
            }

            var pagePathWithoutQueryString = link.GetLeftPart(UriPartial.Path);
            return newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{newQueryString.ToQueryString()}"
                : pagePathWithoutQueryString;
        }

        public static string RemoveQueryString(this Uri link, string queryString, string value = "")
        {
            var newQueryString = link.Query.ParseQueryString();
            newQueryString.Remove("id");
            newQueryString.Remove("page");
            newQueryString.Remove("epslanguage");
            newQueryString.Remove("p");

            if (string.IsNullOrEmpty(value))
            {
                newQueryString.Remove(queryString);
            }

            else
            {
                var values = newQueryString.Get(queryString);
                if (!string.IsNullOrEmpty(values))
                {
                    var valueList = values.Split(',').ToList();
                    valueList.Remove(value);
                    if (valueList.Any())
                        newQueryString.Set(queryString, string.Join(",", valueList));
                    else
                        newQueryString.Remove(queryString);
                }
            }

            var pagePathWithoutQueryString = link.GetLeftPart(UriPartial.Path);
            return newQueryString.Count > 0
                ? $"{pagePathWithoutQueryString}?{newQueryString.ToQueryString()}"
                : pagePathWithoutQueryString;
        }

        public static string FilterResult(this Uri link, string type, string filter, bool append = false)
        {
            if (!string.IsNullOrWhiteSpace(type))
                return !string.Equals(filter, "#")
                    ? AddQueryString(link, type, filter, append)
                    : link.RemoveQueryString(type);
            return link.RemoveQueryString("");
        }

        public static bool IsWellFormedAsAbsoluteUrl(this Uri relativeUri)
        {
            if (relativeUri == null)
            {
                return false;
            }

            var isWellFormed = relativeUri.IsWellFormedOriginalString();
            var baseUri = EPiServer.Web.SiteDefinition.Current?.SiteUrl;
            if (baseUri == null)
            {
                return isWellFormed;
            }

            return !Uri.TryCreate(baseUri, relativeUri, out var absoluteUri)
                ? isWellFormed
                : absoluteUri.IsWellFormedOriginalString();
        }

        public static NameValueCollection ParseQueryString(this string queryStr)
        {
            if (queryStr == null)
            {
                throw new ArgumentNullException(nameof(queryStr));
            }

            var result = new NameValueCollection();
            var queryLength = queryStr.Length;
            var startPos = queryStr.LastIndexOf('?');

            var namePos = queryLength > 0 && startPos > -1 ? startPos + 1 : 0;
            if (queryLength == namePos)
            {
                return result;
            }

            while (namePos <= queryLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (var q = namePos; q < queryLength; q++)
                {
                    if (valuePos == -1 && queryStr[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (queryStr[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                string name;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = WebUtility.UrlDecode(queryStr.Substring(namePos, valuePos - namePos - 1));
                }

                if (valueEnd < 0)
                {
                    valueEnd = queryStr.Length;
                }

                namePos = valueEnd + 1;
                var value = WebUtility.UrlDecode(queryStr.Substring(valuePos, valueEnd - valuePos));
                result.Add(name, value);
            }

            return result;
        }

        public static string AppendToUrl(this string baseUrl, params string[] segments)
                                    => string.Join("/", new[] { baseUrl.TrimEnd('/') }
                                             .Concat(segments.Select(s => s.Trim('/'))));

        public static Uri Append(this Uri uri, params string[] segments)
                                    => new(uri.AppendToAbsoluteUriString(segments));

        public static string AppendToAbsoluteUriString(this Uri uri, string[] segments)
                                    => uri.AbsoluteUri.AppendToUrl(segments);

        public static string AppendToPath(this string basePath, params string[] segments)
                                    => string.Join(Path.DirectorySeparatorChar.ToString(),
                                                   new[] { basePath.TrimEnd(Path.DirectorySeparatorChar) }
                                             .Concat(segments.Select(s => s.Trim(Path.DirectorySeparatorChar))));
    }
}

