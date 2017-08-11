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
            var umbracoContext = umbracoHelper.UmbracoContext;
            var homepage = content.AncestorOrSelf(1);
            //If it's a nested content element or the 'website settings' node, it has no knowledge of the home page and we need to get it some other way.
            if (!homepage.HasProperty("websiteSettings"))
            {
                var currentPage = umbracoContext.ContentCache.GetById(umbracoContext.PageId.Value);
                homepage = currentPage.AncestorOrSelf(1);
            }
            //It can be a page inside the website components folder rendering by itself.
            if (!homepage.HasProperty("websiteSettings"))
            {
                var currentPage = umbracoContext.ContentCache.GetById(umbracoContext.PageId.Value);
                // the website components folder may not be at the root such as when there are multiple websites.
                var websiteComponents = currentPage.AncestorsOrSelf().FirstOrDefault(c => c.DocumentTypeAlias.ToLower().Contains("websitecomponents"));
                if (websiteComponents != null)
                {
                    var rootNodes = umbracoHelper.TypedContentAtRoot();
                    homepage = rootNodes.FirstOrDefault(c => c.HasValue("websiteComponents") && c.GetLink("websiteComponents").Id == websiteComponents.Id);
                }
            }
            var link = homepage.GetLink("websiteSettings");
            return umbracoHelper.TypedContent(link.Id);
        }
    }
}
