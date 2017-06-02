using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Collections
    {

        /// <summary>
        /// (typed) Use for NestedContent properties 
        /// </summary>
        public static IEnumerable<IPublishedContent> GetNestedContent(this IPublishedContent content, string property, bool recursive = false)
        {
            var list = content.GetPropertyValue<IEnumerable<IPublishedContent>>(property, recursive);
            return list ?? Enumerable.Empty<IPublishedContent>();
        }

        /// <summary>
        /// (typed) Works with Multinode Treepicker. 
        /// </summary>
        public static IEnumerable<IPublishedContent> GetNodesTyped(this IPublishedContent content, string property, bool recursive = false)
        {
            var items = content.GetPropertyValue<IEnumerable<IPublishedContent>>(property, recursive);
            return items;
        }

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

        /// <summary>
        /// Splits a list into several lists of roughly the same size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            int i = 0;
            var splits = from item in list
                         group item by i++ % parts into part
                         select part.AsEnumerable();
            return splits;
        }
    }
}
