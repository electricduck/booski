namespace Booski.Utilities;
using System.Text;
using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Data;

public class XUtilities
{
    static readonly int XPostTextLimit = 280;

    public static async Task<string> GeneratePostText(Post post)
    {
        // NOTE: We're parsing the mention facet just to make replacements easier
        /*string captionText = BskyUtilities.ParseFacets(
            post.Record.Text,
            post.Record.Facets,
            linkStringStart: "[",
            linkStringEnd: "]([uri])",
            mentionStringStart: "[",
            mentionStringEnd: "](https://bsky.app/profile/[did])",
            tagStringStart: "",
            tagStringEnd: ""
        );*/
        string captionText = "";
        captionText = await ReplaceUsernames(captionText);
        captionText = UnTruncateLinks(captionText);
        string readMoreLink = $"{Environment.NewLine}—{Environment.NewLine}➡️ https://bsky.app/profile/{post.Profile.Did}/post/{post.RecordKey}";
        int captionTextLength = Encoding.UTF8.GetBytes(captionText).Length;
        int readMoreLinkLength = Encoding.UTF8.GetBytes(readMoreLink).Length;

        if(captionTextLength > XPostTextLimit)
        {
            captionText = StringUtilities.Truncate(captionText, XPostTextLimit - readMoreLinkLength) + readMoreLink;
        }

        return captionText;
    }

    static string UnTruncateLinks(string originalString)
    {
        string pattern = "(\\[.*?\\]\\((.*?)\\))";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string originalLink = match.Value;
            if(match.Groups[2] != null)
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

    static async Task<string> ReplaceUsernames(string originalString)
    {
        string pattern = "(\\[@.*?\\]\\(https:\\/\\/bsky.app\\/profile\\/(.*?)\\))";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string href = match.Value;
            if(match.Groups[2] != null)
            {
                string did = match.Groups[2].Value;
                string xHandle = await UsernameMaps.GetXHandleForDid(did);

                if (!String.IsNullOrEmpty(xHandle))
                {
                    originalString = originalString.Replace(
                        href,
                        $"@{xHandle}"
                    );
                }
            }
        }

        return originalString;
    }
}