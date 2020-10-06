using Newtonsoft.Json;

namespace Ancestor.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object input)
        {
            return JsonConvert.SerializeObject(input, Formatting.Indented);
        }

        public static T FromJson<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
