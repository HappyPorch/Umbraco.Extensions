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
        ///  returns empty List of Link if property not found or empty 
        /// </summary>
        public static IEnumerable<Link> GetLinks(this IPublishedContent content, string property, bool recursive = false)
        {
            var list = content.GetPropertyValue<IEnumerable<Link>>(property, recursive);
            //as of v2.0.0 RJP.MultiURLPicker returns a null IEnumerable when max number of items set to 1
            //  so to support both in this method we need to check for null and if so check for a simgle Link
            if (list == null)
            {
                var singleUrl = content.GetPropertyValue<Link>(property, recursive);
                list = (singleUrl==null) ? Enumerable.Empty<Link>() : new List<Link> { singleUrl };
            }
            return list;
        }

        /// <summary>
        /// (typed) Works for SingleUrlPicker and MultiUrlPicker. Returns the first link.
        /// </summary>
        public static Link GetLink(this IPublishedContent content, string property, bool recursive = false)
        {
            var list = content.GetLinks(property, recursive);
            return list.FirstOrDefault();
        }

        /// <summary>
        /// Gets the links as a collection of IPublishedContent.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetLinksContent(this IPublishedContent content, string property, bool recursive = false)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var links = content.GetLinks(property, recursive);
            var linksContent = new List<IPublishedContent>();
            foreach (var link in links)
            {
                if (link != null && link.Type == LinkType.Content)
                {
                    linksContent.Add(umbracoHelper.TypedContent(link.Id));
                }
            }
            return linksContent;
        }

        /// <summary>
        /// Gets the link as IPublishedContent. Check for null.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static IPublishedContent GetLinkContent(this IPublishedContent content, string property, bool recursive = false)
        {
            var linksContent = content.GetLinksContent(property, recursive);
            return linksContent.FirstOrDefault();
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
        /// <param name="content"></param>
        /// <returns></returns>
        public static IHtmlString ShowLinks(this IPublishedContent item, string property = "link", string extraClass = "", bool recurse = false, string prepend = "", string append = "", string content = "")
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
                        htmlResult.Append($"<a href=\"{link.Url}\" target=\"{link.Target}\" class=\"{extraClass} {currentPage}\">{(content == "" ? link.Name : content)}</a>");
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

        /// <summary>
        /// Works for SingleUrlPicker. Shows the href and target attributes of the link.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static IHtmlString ShowLinkWithoutTags(this IPublishedContent item, string property = "link", bool recursive = false)
        {
            if (item != null && item.HasValue(property, recursive))
            {
                var links = item.GetPropertyValue<MultiUrls>(property, recursive);
                if (links != null && links.Any())
                {
                    var link = links.First();
                    var markup = $"href=\"{link.Url}\" target=\"{link.Target}\"";
                    return new HtmlString(markup);
                }
            }
            var markup2 = $"href=\"#\"";
            return new HtmlString(markup2);
        }
    }
}
