using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Endzone.Umbraco.Extensions.PublishedContentExtensions
{
    public static class Media
    {

        public static IEnumerable<IPublishedContent> GetMultipleTypedMedia(this IPublishedContent content, string property)
        {
            if (!content.HasValue(property))
                return Enumerable.Empty<IPublishedContent>();

            var imageIds = content.GetPropertyValue<string>(property).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            return umbracoHelper.TypedMedia(imageIds);
        }

        public static IPublishedContent GetMedia(this IPublishedContent content, string property, bool recursive = false)
        {
            var id = content.GetPropertyValue<string>(property, recursive);

            if (id == null)
                return null;

            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
            return umbracoHelper.TypedMedia(id);
        }

        /// <summary>
        /// Works for MultipleMediaPicker where the image media is of type Image Cropper (as opposed to upload). Crops the images to the size specified in the crop.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cropAlias"></param>
        /// <param name="property"></param>
        /// <param name="imgclass"></param>
        /// <param name="recurse"></param>
        /// <param name="lazy"></param>
        /// <param name="urlAppend"></param>
        /// <param name="id"></param>
        /// <param name="prepend"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static IHtmlString ShowImagesCropped(this IPublishedContent item, string cropAlias, string property = "image", string imgclass = "", bool recurse = false, bool lazy = false, string urlAppend = null, string id = null, string prepend = null, string append = null)
        {
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse: recurse))
            {
                var imagesList = item.GetPropertyValue<string>(property, recurse: recurse).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                var imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);
                var attribute = lazy ? "data-lazy" : "src";

                foreach (var imageItem in imagesCollection)
                {
                    var url = imageItem.GetCropUrl(cropAlias: cropAlias, imageCropMode: ImageCropMode.Crop, useCropDimensions: true) + urlAppend;
                    htmlResult.Append(prepend);
                    htmlResult.Append($"<img {attribute}=\"{url}\" id=\"{id}\" class=\"{imgclass}\" alt=\"{imageItem.GetPropertyValue("altText")}\" title=\"{imageItem.GetPropertyValue("altText")}\" />");
                    htmlResult.Append(append);
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for MultipleMediaPicker. Shows images in their original size.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="imgclass"></param>
        /// <param name="recurse"></param>
        /// <param name="lazy"></param>
        /// <param name="urlAppend"></param>
        /// <param name="id"></param>
        /// <param name="prepend"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static IHtmlString ShowImages(this IPublishedContent item, string property = "image", string imgclass = "", bool recurse = false, bool lazy = false, string urlAppend = null, string id = null, string prepend = null, string append = null)
        {
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse: recurse))
            {
                var imagesList = item.GetPropertyValue<string>(property, recurse: recurse).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                var imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);
                var attribute = lazy ? "data-lazy" : "src";

                foreach (var imageItem in imagesCollection)
                {
                    var url = imageItem.Url + urlAppend;
                    htmlResult.Append(prepend);
                    htmlResult.Append($"<img {attribute}=\"{url}\" id=\"{id}\" class=\"{imgclass}\" alt=\"{imageItem.GetPropertyValue("altText")}\" title=\"{imageItem.GetPropertyValue("altText")}\" />");
                    htmlResult.Append(append);
                }
            }
            return new HtmlString(htmlResult.ToString());
        }

        /// <summary>
        /// Works for MultipleMediaPicker where the image media is of type Image Cropper(as opposed to upload). Shows the image's urls in their cropped version.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cropAlias"></param>
        /// <param name="property"></param>
        /// <param name="recurse"></param>
        /// <param name="prepend"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static IHtmlString ShowImageUrlsCropped(this IPublishedContent item, string cropAlias, string property = "image", bool recurse = false, string prepend = null, string append = null)
        {
            var htmlResult = new StringBuilder();
            if (item.HasValue(property, recurse: recurse))
            {
                var imagesList = item.GetPropertyValue<string>(property, recurse: recurse).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                var imagesCollection = umbracoHelper.TypedMedia(imagesList).Where(x => x != null);

                foreach (var image in imagesCollection)
                {
                    var url = prepend + image.GetCropUrl(cropAlias: cropAlias, imageCropMode: ImageCropMode.Crop, useCropDimensions: true) + append;
                    htmlResult.Append(url);
                }
            }
            return new HtmlString(htmlResult.ToString());
        }
    }
}
