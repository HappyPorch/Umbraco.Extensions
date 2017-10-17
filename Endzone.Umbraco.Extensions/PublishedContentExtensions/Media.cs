using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Media
    {
        //default values
        private const int ImageQuality = 100;

        public static IEnumerable<IPublishedContent> GetMultipleTypedMedia(this IPublishedContent content, string property)
        {
            if (!content.HasValue(property))
                return Enumerable.Empty<IPublishedContent>();

            var contentValue = content.GetPropertyValue(property);

            if (contentValue is IEnumerable<IPublishedContent>)
            {
                return (IEnumerable<IPublishedContent>)contentValue;
            }
            else if (contentValue is string)
            {
                // use old comma separated list
                var imageIds = contentValue.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                return umbracoHelper.TypedMedia(imageIds);
            }

            // fallback in case value was neither a list or string
            return Enumerable.Empty<IPublishedContent>();
        }

        public static IPublishedContent GetMedia(this IPublishedContent content, string property, bool recursive = false)
        {
            var contentValue = content.GetPropertyValue(property, recursive);

            if (contentValue is IPublishedContent)
            {
                return (IPublishedContent)contentValue;
            }
            else if (contentValue is string)
            {
                // use old comma separated list
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                return umbracoHelper.TypedMedia(contentValue.ToString());
            }

            // fallback in case value was neither IPublishedContent or string
            return null;
        }

        /// <summary>
        /// Works for MultipleMediaPicker where the image media is of type Image Cropper (as opposed to upload). Crops the images to the size specified in the crop.
        /// </summary>
        public static IHtmlString ShowImagesCropped(this IPublishedContent item, string cropAlias, string property = "image", string imgclass = "", bool recurse = false, bool lazy = false, string urlAppend = null, string id = null, string prepend = null, string append = null, string altTextProperty = "altText")
        {
            var imageQuality = GetWebsiteImageQuality(item);
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse))
            {
                IEnumerable<IPublishedContent> imagesCollection = null;

                var itemValue = item.GetPropertyValue(property, recurse);

                if (itemValue is IEnumerable<IPublishedContent>)
                {
                    // multiple media items
                    imagesCollection = (IEnumerable<IPublishedContent>)itemValue;
                }
                else if (itemValue is IPublishedContent)
                {
                    // single media item
                    imagesCollection = ((IPublishedContent)itemValue).AsEnumerableOfOne();
                }
                else if (itemValue is string)
                {
                    // use old comma separated list
                    var imagesList = itemValue.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                    var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);
                }

                if (imagesCollection != null)
                {
                    var attribute = lazy ? "data-lazy" : "src";

                    foreach (var imageItem in imagesCollection)
                    {
                        var url = imageItem.GetCropUrl(cropAlias: cropAlias, imageCropMode: ImageCropMode.Crop, useCropDimensions: true, quality: imageQuality) + urlAppend;
                        htmlResult.Append(prepend);
                        htmlResult.Append($"<img {attribute}=\"{url}\" id=\"{id}\" class=\"{imgclass}\" alt=\"{imageItem.GetPropertyValue(altTextProperty)}\" title=\"{imageItem.GetPropertyValue(altTextProperty)}\" />");
                        htmlResult.Append(append);
                    }
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for MultipleMediaPicker. Shows images in their original size.
        /// </summary>
        public static IHtmlString ShowImages(this IPublishedContent item, string property = "image", string imgclass = "", bool recurse = false, bool lazy = false, string urlAppend = null, string id = null, string prepend = null, string append = null, string altTextProperty = "altText")
        {
            var imageQuality = GetWebsiteImageQuality(item);
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse))
            {
                IEnumerable<IPublishedContent> imagesCollection = null;

                var itemValue = item.GetPropertyValue(property, recurse);

                if (itemValue is IEnumerable<IPublishedContent>)
                {
                    // multiple media items
                    imagesCollection = (IEnumerable<IPublishedContent>)itemValue;
                }
                else if (itemValue is IPublishedContent)
                {
                    // single media item
                    imagesCollection = ((IPublishedContent)itemValue).AsEnumerableOfOne();
                }
                else if (itemValue is string)
                {
                    // use old comma separated list
                    var imagesList = itemValue.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                    var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);
                }

                if (imagesCollection != null)
                {
                    var attribute = lazy ? "data-lazy" : "src";

                    foreach (var imageItem in imagesCollection)
                    {
                        var url = imageItem.Url + urlAppend;
                        url = url.SetUrlParameter("quality", imageQuality);
                        htmlResult.Append(prepend);
                        htmlResult.Append($"<img {attribute}=\"{url}\" id=\"{id}\" class=\"{imgclass}\" alt=\"{imageItem.GetPropertyValue(altTextProperty)}\" title=\"{imageItem.GetPropertyValue(altTextProperty)}\" />");
                        htmlResult.Append(append);
                    }
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for MultipleMediaPicker where the image media is of type Image Cropper(as opposed to upload). Shows the image's urls in their cropped version.
        /// </summary>
        public static IHtmlString ShowImageUrlsCropped(this IPublishedContent item, string cropAlias, string property = "image", bool recurse = false, string prepend = null, string append = null)
        {
            var imageQuality = GetWebsiteImageQuality(item);
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse))
            {
                IEnumerable<IPublishedContent> imagesCollection = null;

                var itemValue = item.GetPropertyValue(property, recurse);

                if (itemValue is IEnumerable<IPublishedContent>)
                {
                    // multiple media items
                    imagesCollection = (IEnumerable<IPublishedContent>)itemValue;
                }
                else if (itemValue is IPublishedContent)
                {
                    // single media item
                    imagesCollection = ((IPublishedContent)itemValue).AsEnumerableOfOne();
                }
                else if (itemValue is string)
                {
                    // use old comma separated list
                    var imagesList = itemValue.ToString().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                    var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                    imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);
                }

                if (imagesCollection != null)
                {
                    foreach (var image in imagesCollection)
                    {
                        var url = prepend + image.GetCropUrl(cropAlias: cropAlias, imageCropMode: ImageCropMode.Crop, useCropDimensions: true, quality: imageQuality) + append;
                        htmlResult.Append(url);
                    }
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        private static int GetWebsiteImageQuality(IPublishedContent item)
        {
            //tries to get it from the website settings node
            var websiteSettings = item.GetWebsiteSettings();
            if (websiteSettings != null && websiteSettings.HasValue("ImageQuality"))
            {
                return websiteSettings.GetPropertyValue<int>("imageQuality");
            }
            // tries to get it from web.config
            var regex = new Regex("^(100|[1-9][0-9]|[1-9])$");
            var imageQuality = ConfigurationManager.AppSettings["websiteImageQuality"];
            if (imageQuality != null && regex.IsMatch(imageQuality))
            {
                return int.Parse(imageQuality);
            }
            return ImageQuality;
        }
    }
}
