using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RJP.MultiUrlPicker.Models;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Menus
    {
        /// <summary>
        /// Displays a menu from a set of links in a MultiUrlPicker.
        /// </summary>
        /// <param name="content">The page with the MultiUrlPicker property.</param>
        /// <param name="property"></param>
        /// <param name="showChildren"></param>
        /// <param name="ulInnerClass"></param>
        /// <param name="addParentToSubMenu"></param>
        /// <returns></returns>
        public static IHtmlString ShowMenuSimple(this IPublishedContent content, string property, bool showChildren = false, string ulInnerClass = null, bool addParentToSubMenu = false)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var links = content.GetLinks(property);
            var markup = new StringBuilder();
            foreach (var link in links)
            {
                if (link == null) { continue; }
                if (link.Type == LinkType.Content)
                {
                    var linkContent = umbracoHelper.TypedContent(link.Id);
                    var linkIsHidden = linkContent != null && linkContent.GetPropertyValue<bool>("umbracoNaviHide");
                    if (linkIsHidden)
                    {
                        continue;
                    }
                    markup.Append($"<li class=\"{(content.Id == link.Id ? "current" : null)}\">");

                    markup.Append($"<a target=\"{link.Target}\" href=\"{link.Url}\">{link.Name}</a>");
                    if (showChildren && linkContent.Children.Any())
                    {
                        markup.Append($"<ul class=\"{ulInnerClass}\">");
                        if (addParentToSubMenu)
                        {
                            markup.Append($"<li><a target=\"{ link.Target}\" href=\"{ link.Url}\">{link.Name}</a></li>");
                        }
                        foreach (var child in linkContent.ChildrenVisible())
                        {
                            var childIsHidden = child.GetPropertyValue<bool>("umbracoNaviHide");
                            if (childIsHidden)
                            {
                                continue;
                            }
                            markup.Append($"<li><a href=\"{child.Url}\">{child.Name}</a></li>");
                        }
                        markup.Append("</ul>");
                    }
                    markup.Append("</li>");
                }
                else if (link.Type == LinkType.External)
                {
                    markup.Append($"<li><a href=\"{link.Url}\">{link.Name}</a></li>");

                }
            }
            return new HtmlString(markup.ToString());
        }

        /// <summary>
        /// Displays a menu from a set of links in a Nested Content property.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <param name="showChildren"></param>
        /// <param name="ulInnerClass"></param>
        /// <param name="addParentToSubMenu"></param>
        /// <param name="useCustomLinkText">Overrides the link text with the `link text` field</param>
        /// <returns></returns>
        public static IHtmlString ShowNestedContentMenu(this IPublishedContent content, string property, bool showChildren = false, string ulInnerClass = null, bool addParentToSubMenu = false, bool useCustomLinkText = false)
        {
            var mainMenu = content.GetNestedContent(property);
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            var umbracoContext = umbracoHelper.UmbracoContext;
            var currentPage = umbracoContext.ContentCache.GetById(umbracoContext.PageId.Value);
            var markup = new StringBuilder();
            foreach (var item in mainMenu)
            {
                var link = item.GetLink("link");
                if (link == null) { continue; }
                if (link.Type == LinkType.Content)
                {
                    var linkContent = umbracoHelper.TypedContent(link.Id);
                    var linkIsHidden = content != null && content.GetPropertyValue<bool>("umbracoNaviHide");
                    if (linkIsHidden)
                    {
                        continue;
                    }
                    var linkText = useCustomLinkText ? item.GetPropertyValue<string>("linkText") : link.Name;
                    markup.Append($"<li class=\"{(currentPage.IsDescendantOrSelf(linkContent) ? "current" : null)}\">");
                    markup.Append($"<a target=\"{link.Target}\" href=\"{link.Url}\">{linkText}</a>");
                    var showLinksChildren = showChildren;
                    if (item.HasValue("showLinksChildren"))
                    {
                        showLinksChildren = item.GetPropertyValue<string>("showLinksChildren").ToLower() == "yes";
                    }
                    if (showLinksChildren)
                    {
                        markup.Append($"<ul class=\"{ulInnerClass}\">");
                        if (item.HasValue("tagsProperty"))
                        {
                            var tagsProperty = item.GetPropertyValue<string>("tagsProperty");
                            var tags = linkContent.ChildrenVisible().GetAllTags(tagsProperty);
                            foreach (var tag in tags)
                            {
                                markup.Append($"<li><a href=\"{linkContent.Url}#{tag.SanitizeTagName()}\">{tag}</a></li>");
                            }
                        }
                        else if (linkContent.ChildrenVisible().Any())
                        {
                            if (addParentToSubMenu)
                            {
                                markup.Append($"<li><a target=\"{link.Target}\" href=\"{link.Url}\">{link.Name}</a></li>");
                            }
                            foreach (var child in linkContent.ChildrenVisible())
                            {
                                markup.Append($"<li class=\"{(currentPage.IsDescendantOrSelf(child) ? "current" : null)}\"><a href=\"{child.Url}\">{child.Name}</a></li>");
                            }
                        }
                        markup.Append("</ul>");
                    }
                    markup.Append("</li>");
                }
                else if (link.Type == LinkType.External)
                {
                    markup.Append($"<li><a href=\"{link.Url}\">{link.Name}</a></li>");
                }
            }
            return new HtmlString(markup.ToString());
        }
    }
}
