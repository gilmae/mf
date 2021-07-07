using System;
namespace mf
{
    public static class UrlExtensions
    {
        public static bool IsRelative(this string url)
        {
            return !new Uri(url, UriKind.RelativeOrAbsolute).IsAbsoluteUri;
        }

        public static string MakeAbsolute(this string url, Uri baseUrl)
        {
            if (!url.IsRelative()) {
                return url;
            }

            return new Uri(baseUrl, url).ToString();
        }
    }
}
