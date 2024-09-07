namespace Booski.Utilities;
using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Data;

public class TelegramUtilities
{
    public static async Task<string> GenerateCaption(Post post)
    {
        string originalCaptionText = BskyUtilities.ParseFacets(
            post.Record.Text,
            post.Record.Facets
        );
        originalCaptionText = await ReplaceUsernames(originalCaptionText);

        string captionText = $"""
<a href="https://bsky.app/profile/{post.Profile.Did}">🦋 @{post.Profile.Handle}</a>
""";

        if (!String.IsNullOrEmpty(originalCaptionText))
        {
            captionText = $"""
{originalCaptionText}
—
{captionText}
""";
        }

        return captionText;
    }

    static async Task<string> ReplaceUsernames(string originalString)
    {
        string pattern = "(<a href=\"https:\\/\\/bsky.app\\/profile\\/(.*?)\">@.*?<\\/a>)";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string href = match.Value;
            if(match.Groups[2] != null)
            {
                string did = match.Groups[2].Value;
                string telegramHandle = await UsernameMaps.GetTelegramHandleForDid(did);

                if (!String.IsNullOrEmpty(telegramHandle))
                {
                    originalString = originalString.Replace(
                        href,
                        $"@{telegramHandle}"
                    );
                }
            }
        }

        return originalString;
    }
}