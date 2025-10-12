using Aco228.Common.Extensions;

namespace Aco228.Common.Helpers;

public static class StringUrlHelper
{
    public static string GetFileName(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;
        
        var usingChar = '/';
        if(!url.Contains("/") && url.Contains(@"\"))
            usingChar = '\\';

        return url.Split("?")[0].GetUntilCharReverse(usingChar);
    }
}