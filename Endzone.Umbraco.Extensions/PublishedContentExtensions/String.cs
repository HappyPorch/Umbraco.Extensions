using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class String
    {

        /// <summary>
        /// (typed) gets the contents of a textarea in a list split by line breaks
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetStringSplitByLines(this IPublishedContent content, string property = "bulletList")
        {
            if (!content.HasValue(property))
                return Enumerable.Empty<string>();
            var list = content.GetPropertyValue<string>(property).Split(new string[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);
            return list ?? Enumerable.Empty<string>();
        }


        /// <summary>
        /// (typed) Works with text separated by a char. Gets the text content as a enumerable of strings.
        /// </summary>
        public static IEnumerable<string> GetStringSplitBySeparator(this IPublishedContent content, string propertyName, string splitChar = ",")
        {
            if (!content.HasValue(propertyName))
                return Enumerable.Empty<string>();
            var splitString = new[] { splitChar };
            var words = content.GetPropertyValue<string>(propertyName).Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            return words;
        }

        /// <summary>
        /// Turns line breaks in regular text into html markup with line breaks
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IHtmlString ShowTextWithLineBreaks(this IPublishedContent content, string property = "text")
        {
            var text = content.GetStringSplitByLines(property);
            var htmlText = string.Join("<br />", text);
            return  new HtmlString(htmlText);
        }

        /// <summary>
        /// Turns lines in regular text into <li> elements
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IHtmlString ShowBulletList(this IPublishedContent content, string property = "text")
        {
            var text = content.GetStringSplitByLines(property);
            var markup = new StringBuilder();
            foreach (var line in text)
            {
                markup.Append($"<li>{line}</li>");
            }
            return new HtmlString(markup.ToString());
        }
    }
}
