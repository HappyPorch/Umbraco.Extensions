using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Endzone.Umbraco.Extensions.PropertyValueConverters
{
    /// <summary>
    /// PropertyValueConverter which converts the value stored for a Switcher data type to a boolean.
    /// </summary>
    [PropertyValueType(typeof(bool))]
    [PropertyValueCache(PropertyCacheValue.All, PropertyCacheLevel.ContentCache)]
    public class SwitcherValueConverter : IPropertyValueConverter
    {
        public bool IsConverter(PublishedPropertyType propertyType)
        {
            return propertyType.PropertyEditorAlias.Equals("Our.Umbraco.Switcher");
        }

        public object ConvertDataToSource(PublishedPropertyType propertyType, object source, bool preview)
        {
            var attemptConvertBool = source.TryConvertTo<bool>();

            if (attemptConvertBool.Success)
            {
                return attemptConvertBool.Result;
            }

            return null;
        }

        public object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source;
        }

        public object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview)
        {
            return source?.ToString();
        }
    }
}
