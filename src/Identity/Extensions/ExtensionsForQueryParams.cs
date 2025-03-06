
using System.Web;

namespace Identity.Extensions
{
    public static class ExtensionsForQueryParams
    {
        public static string ToQueryParams(this Dictionary<string, string> items, bool escape)
        {
            return string.Join("&", items.Keys.Select(key =>
             {
                 if (escape)
                 {
                     return $"{key}={HttpUtility.UrlEncode(items[key])}";
                 }
                 else
                 {
                     return $"{key}={items[key]}";
                 }
             }));
        }

        public static string ToFormFields(this Dictionary<string, string> items)
        {
            return string.Concat(items.Keys.Select(key =>
            {
                // TODO: Handle escaping
                return $"<input type=\"hidden\" name=\"{key}\" value=\"{items[key]}\"/>";
            }));
        }
    }
}
