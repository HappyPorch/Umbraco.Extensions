using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Configuration
    {
        /// <summary>
        /// Returns the website settings node
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IPublishedContent GetWebsiteSettings(this IPublishedContent content)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var homepage = content.AncestorOrSelf(1);
            var link = homepage.GetLink("websiteSettings");
            return umbracoHelper.TypedContent(link.Id);
        }
    }
}
