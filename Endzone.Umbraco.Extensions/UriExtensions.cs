using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Endzone.Umbraco.Extensions
{
    public static class UriExtensions
    {

        /// <summary>
        /// Sets a parameter in the url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SetUrlParameter(this string url, string paramName, object value)
        {
            return new Uri(url).SetParameter(paramName, value.ToString()).ToString();
        }

        private static Uri SetParameter(this Uri url, string paramName, string value)
        {
            var queryParts = HttpUtility.ParseQueryString(url.Query);
            queryParts[paramName] = value;
            return new Uri(url.AbsoluteUriExcludingQuery() + '?' + queryParts.ToString());
        }

        private static string AbsoluteUriExcludingQuery(this Uri url)
        {
            return url.AbsoluteUri.Split('?').FirstOrDefault() ?? String.Empty;
        }
    }
}
