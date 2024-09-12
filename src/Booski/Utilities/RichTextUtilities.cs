using System.Text.RegularExpressions;

namespace Booski.Utilities;

public static class RichTextUtilities
{
    public static string UnTruncateMarkdownLinks(string originalString)
    {
        string pattern = "(\\[.*?\\]\\((.*?)\\))";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string originalLink = match.Value;
            if (match.Groups[2] != null)
            {
                string originalUrl = match.Groups[2].Value;

                originalString = originalString.Replace(
                    originalLink,
                    originalUrl
                );
            }
        }

        return originalString;
    }
}