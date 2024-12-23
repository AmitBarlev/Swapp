using System.Collections.Specialized;
using System.Web;

namespace Swap.Object.Tools
{
    public static class Extensions
    {
        public static bool TryGetValue(this Microsoft.AspNetCore.Http.HttpRequest httpRequest, string key, out int value)
        {
            value = 0;
            if (!httpRequest.QueryString.HasValue)
                return false;

            NameValueCollection queryParams = HttpUtility.ParseQueryString(httpRequest.QueryString.Value);
            return int.TryParse(queryParams[key], out value);
        }

        public static bool TryGetToken(this Microsoft.AspNetCore.Http.HttpRequest httpRequest, out string token)
        {
            const string tokenKey = "Authorization";
            token = null;
            if (!httpRequest.Headers.ContainsKey(tokenKey))
                return false;

            token = httpRequest.Headers[tokenKey].ToString().Split(' ')[1];
            return true;
        }

        public static bool TryGetValue(this NameValueCollection nameValueCollection, string key, out string value)
        {
            value = null;
            if (null == nameValueCollection[key])
                return false;

            value = nameValueCollection[key];
            return true;
        }

        public static bool TryGetValue(this NameValueCollection nameValueCollection, string key, out int value)
        {
            return int.TryParse(nameValueCollection[key], out value);
        }
    }
}
