using Newtonsoft.Json;

namespace Ancestor.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object input, Formatting formatting = Formatting.Indented)
        {
            return JsonConvert.SerializeObject(input, formatting);
        }

        public static T FromJson<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
