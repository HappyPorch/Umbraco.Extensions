using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Endzone.Umbraco.Extensions
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Sets a query parameter in the url
        /// </summary>
        public static string SetUrlParameter(this string url, string paramName, object value)
        {
            var regex = new Regex("^(?<base>[^?#]*)?(?<query>[?][^#]*)?(?<hash>[#].*)?$");
            var match = regex.Match(url);

            if (!match.Success)
                return url;

            var baseUrl = match.Groups["base"].Value;
            var queryPart = match.Groups["query"].Value;
            var hash = match.Groups["hash"].Value;

            var queryParts = HttpUtility.ParseQueryString(queryPart);
            queryParts[paramName] = value.ToString();

            return baseUrl + '?' + queryParts + hash;
        }

        /// <summary>
        /// Turns relative URL into an absolute URL using the current request host.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToAbsoluteUrl(this string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                // empty URL
                return url;
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                // URL is not relative
                return url;
            }

            var httpContext = HttpContext.Current;

            if (httpContext?.Request?.Url == null)
            {
                // HttpContext or request URL are missing
                return url;
            }

            var requestHostUri = new Uri(httpContext.Request.Url.GetLeftPart(UriPartial.Authority));

            var absoluteUri = new Uri(requestHostUri, url);

            return absoluteUri.ToString();
        }
    }
}
