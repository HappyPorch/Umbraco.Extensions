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
            //If it's a nested content element or the 'website settings' node, it has no knowledge of the home page and we need to get it some other way.
            if (content.Level == 0 || content.DocumentTypeAlias.ToLower() == "websitesettings")
            {
                var umbracoContext = umbracoHelper.UmbracoContext;
                var currentPage = umbracoContext.ContentCache.GetById(umbracoContext.PageId.Value);
                homepage = currentPage.AncestorOrSelf(1);
            }
            var link = homepage.GetLink("websiteSettings");
            return umbracoHelper.TypedContent(link.Id);
        }
    }
}
