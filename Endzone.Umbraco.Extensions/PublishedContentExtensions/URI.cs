using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class URI
    {
        public static string AddParameterToUrl(this string url, string paramName, string paramValue)
        {
            if (url.Contains("?"))
            {
                return url + "&" + paramName + "=" + paramValue;
            }
            return url + "?" + paramName + "=" + paramValue;
        }
    }
}
