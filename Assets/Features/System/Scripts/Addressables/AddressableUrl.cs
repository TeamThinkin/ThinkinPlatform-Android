using System;
using Thinkin.Web;

public struct AddressableUrl
{
    public Uri Parsed;
    public string CatalogUrl;
    public string AssetPath;

    public AddressableUrl(string url)
    {
        Parsed = new Uri(url.Replace("{PLATFORM}", Config.PlatformKey));
        CatalogUrl = $"{Parsed.Scheme}://{Parsed.Authority}{Parsed.PathAndQuery}";
        //AssetPath = WebSocketSharp.Net.HttpUtility.UrlDecode(uri.Fragment.Substring(1));
        AssetPath = HttpUtility.UrlDecode(Parsed.Fragment.Substring(1));
    }
}
