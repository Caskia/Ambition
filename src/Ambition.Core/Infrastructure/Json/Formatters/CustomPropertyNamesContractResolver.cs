using Ambition.Core.Utils;
using Newtonsoft.Json.Serialization;

namespace Ambition.Core.Json.Formatters
{
    public class CustomPropertyNamesContractResolver : DefaultContractResolver
    {
        public CustomPropertyNamesContractResolver()
        { }

        protected override string ResolvePropertyName(string propertyName)
        {
            return StringUtils.ToCustomSeparatedCase(propertyName);
        }
    }
}