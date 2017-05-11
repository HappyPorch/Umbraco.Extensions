﻿using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class ContentTree
    {

        /// <summary>
        /// Gets all visible children (using umbracoNaviHide)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> ChildrenVisible(this IPublishedContent content, string DocumentTypeAlias = null)
        {
            var children = content.Children.Where(o => o.GetPropertyValue<bool>("umbracoNaviHide") != true);
            if (DocumentTypeAlias != null)
            {
                children = children.Where(d => d.DocumentTypeAlias == DocumentTypeAlias);
            }
            return children;
        }

        /// <summary>
        /// Gets all visible descendants (using umbracoNaviHide)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="DocumentTypeAlias">optional</param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> DescendantsVisible(this IPublishedContent content, string DocumentTypeAlias = null)
        {
            var descendants = content.Descendants().Where(o => o.GetPropertyValue<bool>("umbracoNaviHide") != true);
            if (DocumentTypeAlias != null)
            {
                descendants = descendants.Where(d => d.DocumentTypeAlias == DocumentTypeAlias);
            }
            return descendants;
        }

        /// <summary>
        /// Gets all visible siblings (using umbracoNaviHide)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="DocumentTypeAlias">optional</param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> SiblingsVisible(this IPublishedContent content, string DocumentTypeAlias = null)
        {
            var siblings = content.Siblings().Where(o => o.GetPropertyValue<bool>("umbracoNaviHide") != true);
            if (DocumentTypeAlias != null)
            {
                siblings = siblings.Where(d => d.DocumentTypeAlias == DocumentTypeAlias);
            }
            return siblings;
        }
    }
}