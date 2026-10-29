namespace Aco228.Common.Extensions;

public static class StringUrlExtensions
{
    public static string EnsureQueryStart(this string url)
    {
        if (string.IsNullOrEmpty(url))
            return url;

        var fragmentIndex = url.IndexOf('#');
        var fragment = fragmentIndex >= 0 ? url[fragmentIndex..] : "";
        var beforeFragment = fragmentIndex >= 0 ? url[..fragmentIndex] : url;

        return beforeFragment.Contains('?')
            ? url
            : $"{beforeFragment}?{fragment}";
    }
    
    /// <param name="query">param=test, or several: a=1&amp;b=2 (a leading '?' is fine). Treated as already encoded.</param>
    public static string AddOrReplaceQueryInUrl(this string url, string query)
    {
        if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(query))
            return url;

        var fragmentIndex = url.IndexOf('#');
        var fragment = fragmentIndex >= 0 ? url[fragmentIndex..] : "";
        if (fragmentIndex >= 0) url = url[..fragmentIndex];

        var queryIndex = url.IndexOf('?');
        var path = queryIndex >= 0 ? url[..queryIndex] : url;
        var existing = queryIndex >= 0 ? url[(queryIndex + 1)..] : "";

        var parts = existing.Split('&', StringSplitOptions.RemoveEmptyEntries).ToList();
        var newParts = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (var newPart in newParts)
        {
            var key = GetKey(newPart);
            if (key.Length == 0) continue;

            var first = parts.FindIndex(p => GetKey(p).Equals(key, StringComparison.OrdinalIgnoreCase));
            parts.RemoveAll(p => GetKey(p).Equals(key, StringComparison.OrdinalIgnoreCase));

            if (first >= 0) parts.Insert(first, newPart);
            else parts.Add(newPart);
        }

        return parts.Count == 0
            ? path + fragment
            : $"{path}?{string.Join("&", parts)}{fragment}";

        static string GetKey(string part)
        {
            var eq = part.IndexOf('=');
            var rawKey = eq >= 0 ? part[..eq] : part;
            return Uri.UnescapeDataString(rawKey.Replace('+', ' '));
        }
    }
}