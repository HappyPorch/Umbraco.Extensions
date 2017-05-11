using RJP.MultiUrlPicker.Models;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class MultiUrlPicker
    {

        /// <summary>
        /// (typed) Works for SingleUrlPicker and MultiUrlPicker 
        /// </summary>
        public static IEnumerable<Link> GetLinks(this IPublishedContent content, string property, bool recursive = false)
        {
            var list = content.GetPropertyValue<IEnumerable<Link>>(property, recursive);
            return list ?? Enumerable.Empty<Link>();
        }

        /// <summary>
        /// (typed) Works for SingleUrlPicker and MultiUrlPicker. Returns the first link.
        /// </summary>
        public static Link GetLink(this IPublishedContent content, string property, bool recursive = false)
        {
            var list = content.GetPropertyValue<IEnumerable<Link>>(property, recursive) ?? Enumerable.Empty<Link>();
            return list.FirstOrDefault();
        }
    }
}
