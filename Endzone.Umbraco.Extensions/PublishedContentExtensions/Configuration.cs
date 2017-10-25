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

        /// <summary>
        /// Gets the strongly typed auxiliary content (e.g. website settings) for the current site.
        /// </summary>
        /// <example>
        /// Homepage node has a composition doc type of AuxiliaryFolder (strongly-typed interface: IAuxiliaryFolder)
        /// which has a property called WebsiteSettings (MNTP limited to doc type WebsiteSettings).
        /// To get the website settings on the current node call: 
        /// Model.GetAuxiliaryContent&lt;IAuxiliaryFolder, WebsiteSettings&gt;(x => x.WebsiteSettings)
        /// This will return the selected WebsiteSettings node with its strongly-typed model.
        /// </example>
        /// <param name="content"></param>
        /// <param name="property">The strongly typed property that contains the MNTP value for the auxiliary content</param>
        /// <returns></returns>
        public static TAuxiliaryType GetAuxiliaryContent<TAuxiliaryFolder, TAuxiliaryType>(this IPublishedContent content, Func<TAuxiliaryFolder, IEnumerable<IPublishedContent>> property)
            where TAuxiliaryFolder : class, IPublishedContent
            where TAuxiliaryType : class, IPublishedContent
        {
            var auxiliaryFolderNode = content?.AncestorOrSelf<TAuxiliaryFolder>();

            if (auxiliaryFolderNode == null)
            {
                if (UmbracoContext.Current?.PageId == null || UmbracoContext.Current.PageId == content?.Id)
                {
                    // no Umbraco context found or it's for the same node we already checked
                    return default(TAuxiliaryType);
                }

                // try to get it based on current page ID, in case 'content' is a Nested Content node.
                var currentPage = UmbracoContext.Current.ContentCache.GetById(UmbracoContext.Current.PageId.Value);

                auxiliaryFolderNode = currentPage?.AncestorOrSelf<TAuxiliaryFolder>();
            }

            if (auxiliaryFolderNode == null)
            {
                return default(TAuxiliaryType);
            }

            return property(auxiliaryFolderNode)?.FirstOrDefault() as TAuxiliaryType;
        }
    }
}
