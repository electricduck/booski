using System.Text;
using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Utilities;
using LinqToTwitter;

namespace Booski.Helpers;

public interface IXHelpers
{
    Task DeleteFromX(
        string postId
    );
    Task<Tweet> PostToX(
        Post post,
        Embed? embed,
        string? replyId = null
    );
}

internal sealed class XHelpers : IXHelpers
{
    private IBskyHelpers _bskyHelpers;
    private IFileCacheContext _fileCacheContext;
    private IXContext _xContext;

    static readonly int XPostTextLimit = 280;

    public XHelpers(
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        IXContext xContext
    )
    {
        _bskyHelpers = bskyHelpers;
        _fileCacheContext = fileCacheContext;
        _xContext = xContext;
    }

    public async Task DeleteFromX(
        string postId
    ) {
        await _xContext.Client.DeleteTweetAsync(postId);
    }

    public async Task<Tweet> PostToX(
        Post post,
        Embed? embed,
        string? replyId = null
    )
    {
        Tweet sentMessage = null;
        List<Media> messageAttachments = new List<Media>();

        if (
            embed != null && embed.Type == Enums.EmbedType.Gif ||
            embed != null && embed.Type == Enums.EmbedType.Images
        )
        {
            if (embed.Items.Count() > 0)
            {
                var httpClient = new HttpClient();

                string mediaCategory = "";

                switch (embed.Type)
                {
                    case Enums.EmbedType.Gif:
                        mediaCategory = "tweet_gif";
                        break;
                    case Enums.EmbedType.Images:
                        mediaCategory = "tweet_image";
                        break;
                    case Enums.EmbedType.Video:
                        mediaCategory = "tweet_video";
                        break;
                }

                foreach (var embedItem in embed.Items)
                {
                    var fileByteArray = await _fileCacheContext.GetFileFromUriAsByteArray(embedItem.Uri);

                    var attachment = await _xContext.Client.UploadMediaAsync(
                        fileByteArray,
                        mediaCategory: mediaCategory,
                        mediaType: embedItem.MimeType
                    );

                    if (attachment != null)
                        messageAttachments.Add(attachment);
                }
            }

            if (messageAttachments != null)
            {
                List<string> mediaIds = new List<string>();

                foreach (var messageAttachment in messageAttachments)
                {
                    mediaIds.Add(messageAttachment.MediaID.ToString());
                }

                // NOTE: There's no "ReplyMediaAsync" function so it will quote for replies
                sentMessage = await _xContext.Client.TweetMediaAsync(
                    mediaIds: mediaIds,
                    quoteTweetID: replyId,
                    text: await GeneratePostText(post)
                );
            }
        }
        else
        {
            if (String.IsNullOrEmpty(replyId))
            {
                sentMessage = await _xContext.Client.TweetAsync(
                    text: await GeneratePostText(post)
                );
            }
            else
            {
                sentMessage = await _xContext.Client.ReplyAsync(
                    replyTweetID: replyId,
                    text: await GeneratePostText(post)
                );
            }
        }

        return sentMessage;
    }

    async Task<string> GeneratePostText(Post post)
    {
        // NOTE: We're parsing the mention facet just to make replacements easier
        string captionText = _bskyHelpers.ParseFacets(
            post.Record.Text,
            post.Record.Facets,
            linkStringStart: "[",
            linkStringEnd: "]([uri])",
            mentionStringStart: "[",
            mentionStringEnd: "](https://bsky.app/profile/[did])",
            tagStringStart: "",
            tagStringEnd: ""
        );
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

    string UnTruncateLinks(string originalString)
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