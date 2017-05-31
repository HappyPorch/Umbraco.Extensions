using System.Web.Mvc.Html;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class PartialViews
    {
        /// <summary>
        /// Renders an IPublishedContent's partial view using the doc type name prepended with "_"
        /// </summary>
        /// <param name="page"></param>
        /// <param name="component"></param>
        /// <param name="folder"></param>
        public static void RenderComponent(this UmbracoTemplatePage page, IPublishedContent component, string folder = null)
        {
            if (component != null)
            {
                var append = folder == null ? "" : ".cshtml";
                page.Html.RenderPartial(folder + "_" + component.DocumentTypeAlias + append, component);
            }
        }
    }
}
