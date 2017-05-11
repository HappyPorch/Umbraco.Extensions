using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Media
    {

        public static IEnumerable<IPublishedContent> GetMultipleTypedMedia(this IPublishedContent content, string propertyName)
        {
            if (!content.HasValue(propertyName))
                return Enumerable.Empty<IPublishedContent>();

            var imageIds = content.GetPropertyValue<string>(propertyName).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            return umbracoHelper.TypedMedia(imageIds);
        }

        public static IPublishedContent GetMedia(this IPublishedContent content, string property, bool recursive = false)
        {
            var id = content.GetPropertyValue<string>(property, recursive);

            if (id == null)
                return null;

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            return umbracoHelper.TypedMedia(id);
        }
    }
}
