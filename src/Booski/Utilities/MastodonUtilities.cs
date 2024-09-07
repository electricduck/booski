namespace Booski.Utilities;
using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Data;

public class MastodonUtilities
{
    public static async Task<string> GenerateStatusText(Post post)
    {
        string statusText = BskyUtilities.ParseFacets(
            post.Record.Text,
            post.Record.Facets,
            linkStringStart: "[",
            linkStringEnd: "]([uri])",
            mentionStringStart: "[",
            mentionStringEnd: "](https://bsky.app/profile/[did])",
            tagStringStart: "",
            tagStringEnd: ""
        );
        statusText = await ReplaceUsernames(statusText);

        return statusText;
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
                string mastodonHandle = await UsernameMaps.GetMastodonHandleForDid(did);

                if (!String.IsNullOrEmpty(mastodonHandle))
                {
                    originalString = originalString.Replace(
                        href,
                        $"@{mastodonHandle}"
                    );
                }
            }
        }

        return originalString;
    }
}