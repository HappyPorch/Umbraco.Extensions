using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class ContentDate
    {

        /// <summary>
        /// Gets the creation date (or from property if supplied) from all the child elements sorted and distinc by month/year
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetAllCreationDates(this IEnumerable<IPublishedContent> content, string property)
        {
            var list = content
                .Select(c => c.GetPropertyValue<DateTime>(property))
                .Select(d => new DateTime(d.Year, d.Month, 1))
                .Distinct()
                .ToList();
            return list;
        }

        /// <summary>
        /// Gets all elements whose publication date is of the same month and year.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="property"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static IEnumerable<IPublishedContent> GetAllElementsWithCreationDate(this IEnumerable<IPublishedContent> content,
            string property, DateTime date)
        {
            var list = content
                .Where(
                    c =>
                        property != null && c.HasProperty(property) &&
                         c.GetPropertyValue<DateTime>(property) != DateTime.MinValue &&
                         c.GetPropertyValue<DateTime>(property).Year == date.Year &&
                         c.GetPropertyValue<DateTime>(property).Month == date.Month);
            return list;
        }
    }
}
