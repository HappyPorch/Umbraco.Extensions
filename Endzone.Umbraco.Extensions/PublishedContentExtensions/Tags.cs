using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Tags
    {

        /// <summary>
        /// Gets all the tags from content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllTags(this IEnumerable<IPublishedContent> content, string property = "tags")
        {
            var taglist = new List<string>();
            if (content != null)
            {
                foreach (var item in content)
                {
                    if (item.HasValue(property))
                    {
                        if (item.GetPropertyValue(property) is string)
                        {
                            taglist.AddRange(item.GetPropertyValue<string>(property).Split(','));
                        }
                        else
                        {
                            taglist.AddRange(item.GetPropertyValue<string[]>(property) ?? Enumerable.Empty<string>());
                        }
                    }
                }
                taglist = taglist.OrderBy(i => i).Distinct().ToList();
            }
            return taglist;
        }

        /// <summary>
        /// Gets all the tags from content's children
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tag"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetElementsWithTag(this IEnumerable<IPublishedContent> content, string tag, string property = "tags")
        {
            var docList = new List<IPublishedContent>();
            if (content != null)
            {
                foreach (var item in content)
                {
                    if (item.HasValue(property))
                    {
                        var tags = item.GetPropertyValue<string>(property).Split(',').Select(i => i.Trim());
                        if (tags.Contains(tag))
                        {
                            docList.Add(item);
                        }
                    }
                }
            }
            return docList;
        }
    }
}
