using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
                    var tags = item.GetPropertyValue(property);
                    if (tags is string s)
                    {
                        taglist.AddRange(s.Split(','));
                    }
                    else if (tags is string[] a)
                    {
                        taglist.AddRange(a);
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

        /// <summary>
        /// Removes unwanted characters from tag name so that it works with javascript plugins
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static string SanitizeTagName(this string tagName)
        {
            tagName = tagName ?? string.Empty;
            Regex rgx = new Regex("[^a-zA-Z, ]");
            return rgx.Replace(tagName, "").ToLower().Replace(" ", "-");
        }
    }
}
