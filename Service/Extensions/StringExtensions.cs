using Newtonsoft.Json;

namespace Service.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
        public static T ToObject<T>(this string value) where T : class
        {
            return string.IsNullOrEmpty(value) ? null : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
