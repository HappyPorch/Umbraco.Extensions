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
    }
}
