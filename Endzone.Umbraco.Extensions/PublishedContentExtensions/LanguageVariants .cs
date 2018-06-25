using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class LanguageVariants
    {
        /// <summary>
        /// Returns an ordered sequence of transaltions of the current page.
        /// When compiling a list of languages, it checks for the cultures of all "home pages" in the root.
        /// In this case the home page document type is the type of the top most ancestor.
        /// </summary>
        /// <param name="content">The page whose translations you want to get.</param>
        /// <returns>An ordered sequence of translations as they appear in the CMS.</returns>
        public static IEnumerable<IPublishedContent> GetLanguageVariants(this IPublishedContent content)
        {
            var root = content.AncestorOrSelf(1);
            return GetLanguageVariants(content, root.DocumentTypeAlias);
        }

        /// <summary>
        /// Returns an ordered sequence of transaltions of the current page.
        /// When compiling a list of languages, it checks for the cultures of all "home pages" in the root.
        /// </summary>
        /// <param name="content">The page whose translations you want to get.</param>
        /// <param name="homePageAlias">The document type alias of the home pages. Will ignore any other document types in the root.</param>
        /// <returns>An ordered sequence of translations as they appear in the CMS.</returns>
        public static IEnumerable<IPublishedContent> GetLanguageVariants(this IPublishedContent content, string homePageAlias)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            //lets start with the defaults - at the very least point to each of the homepages for each language
            var translations = umbracoHelper.TypedContentAtRoot().DescendantsOrSelf(homePageAlias)
                .ToDictionary(p => p.GetCulture());

            FindVariantsForTranslations(content, translations, umbracoHelper);

            if (content.Level > 1 && content.DocumentTypeAlias != homePageAlias)
            {
                // for every translation without a variant, let's find out if there is a variant based on other culture's relations
                var missingVariants = translations.Where(i => i.Value.Level == 1).ToList();
                var foundVariants = translations.Where(i => i.Value.Level > 1).ToList();

                foreach (var mv in missingVariants)
                {
                    foreach (var fv in foundVariants)
                    {
                        FindVariantsForTranslations(fv.Value, translations, umbracoHelper, mv.Value.GetCulture());
                    }
                }
            }

            //if some translation

            return translations.Select(p => p.Value).OrderBy(p => p.AncestorOrSelf(1).SortOrder).ThenBy(p => p.SortOrder);
        }

        private static void FindVariantsForTranslations(IPublishedContent content, Dictionary<CultureInfo, IPublishedContent> translations, UmbracoHelper umbracoHelper, CultureInfo cultureInfo = null)
        {
            //using relation name here to filter & not alias (as the relation has a confusing alias for some reason)
            var relations = ApplicationContext.Current.Services.RelationService.GetByParentOrChildId(content.Id).Where(r => r.RelationType.Name == "Translations");

            foreach (var item in relations)
            {
                var relatedId = item.ChildId != content.Id ? item.ChildId : item.ParentId;
                var variant = umbracoHelper.TypedContent(relatedId);
                if (variant == null) // not published
                    continue;
                var culture = variant.GetCulture();

                // find the variants only for the culture we are looking for
                if (cultureInfo != null && !culture.Equals(cultureInfo))
                {
                    continue;
                }
                translations[culture] = variant;
            }
        }

        /// <summary>
        /// Returns an ordered sequence of translations of the current page suitable for using on the link alternate tags.
        /// This is a simple version of GetLanguageVariants that only returns the relations that include this page.
        /// </summary>
        /// <param name="page">The page whose translations you want to get.</param>
        /// <returns>An ordered sequence of translations as they appear in the CMS.</returns>
        public static IEnumerable<IPublishedContent> GetHrefLangPages(this IPublishedContent page)
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            return ApplicationContext.Current.Services.RelationService.GetByParentOrChildId(page.Id)
                .Where(r => r.RelationType.Name == "Translations")
                .Select(r => r.ChildId != page.Id ? r.ChildId : r.ParentId)                                 // get the id of the related pages
                .Select(i => ApplicationContext.Current.Services.RelationService.GetByParentOrChildId(i))   // get the ids of 2nd level relations
                .SelectMany(i => i)                                                                         // puts them all in one list
                .Select(r => umbracoHelper.TypedContent(r.ChildId != page.Id ? r.ChildId : r.ParentId))     // convert to IPublishedContent
                .Where(p => p != null)                                                                      // remove unpublished pages
                .DistinctBy(p => p.Id);                                                                     // remove duplicates
        }
    }
}
