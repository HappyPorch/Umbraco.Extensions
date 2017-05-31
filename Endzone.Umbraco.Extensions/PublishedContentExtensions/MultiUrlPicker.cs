using RJP.MultiUrlPicker.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
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

        /// <summary>
        /// Works for SingleUrlPicker and MultiUrlPicker. Shows the full html link or collection of links.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="extraClass"></param>
        /// <param name="recurse"></param>
        /// <param name="prepend"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static IHtmlString ShowLinks(this IPublishedContent item, string property = "link", string extraClass = "", bool recurse = false, string prepend = "", string append = "")
        {
            var htmlResult = new StringBuilder();
            if (item != null && item.HasValue(property, recurse))
            {
                var links = item.GetLinks(property, recurse);
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        var currentPage = item.Id == link.Id ? "current" : string.Empty;
                        htmlResult.Append(prepend);
                        htmlResult.Append($"<a href=\"{link.Url}\" target=\"{link.Target}\" class=\"{extraClass} {currentPage}\">{link.Name}</a>");
                        htmlResult.Append(append);
                    }
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for SingleUrlPicker and MultiUrlPicker. Only shows the url of a link or collection of links.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="extraClass"></param>
        /// <param name="recurse"></param>
        /// <param name="prepend"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static IHtmlString ShowLinkUrls(this IPublishedContent item, string property = "link", string extraClass = "", bool recurse = false, string prepend = "", string append = "")
        {
            var htmlResult = new StringBuilder();
            if (item != null && item.HasValue(property, recurse))
            {
                var links = item.GetLinks(property, recurse);
                if (links != null)
                {
                    foreach (var link in links)
                    {
                        var currentPage = item.Id == link.Id ? "current" : string.Empty;
                        htmlResult.Append(prepend);
                        htmlResult.Append(link.Url);
                        htmlResult.Append(append);
                    }
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for SingleUrlPicker. Adds a link around some content. Will show only the content if link is not set.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <param name="extraClass"></param>
        /// <param name="recurse"></param>
        public static IHtmlString SurroundWithLink(this IPublishedContent item, IHtmlString content, string property = "link", string extraClass = "", bool recurse = false)
        {
            if (item != null && item.HasValue(property, recurse))
            {
                var link = item.GetLink(property, recurse);
                if (link != null)
                {
                    var markup = $"<a href=\"{ link.Url}\" target=\"{ link.Target}\" class=\"{ extraClass}\">{content}</a>";
                    return new HtmlString(markup);
                }
            }
            return content;
        }
    }
}
