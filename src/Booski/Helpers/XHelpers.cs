using System.Text;
using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Enums;
using Booski.Utilities;
using LinqToTwitter;

namespace Booski.Helpers;

public interface IXHelpers
{
    Task DeleteFromX(
        string postId
    );
    Task<Tweet?> PostToX(
        Post post,
        Embed? embed,
        string? replyId = null
    );
}

internal sealed class XHelpers : IXHelpers
{
    private IBskyHelpers _bskyHelpers;
    private IFileCacheContext _fileCacheContext;
    private II18nHelpers _i18nHelpers;
    private IXContext _xContext;

    static readonly int XPostTextLimit = 280;

    public XHelpers(
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        II18nHelpers i18nHelpers,
        IXContext xContext
    )
    {
        _bskyHelpers = bskyHelpers;
        _fileCacheContext = fileCacheContext;
        _i18nHelpers = i18nHelpers;
        _xContext = xContext;
    }

    public async Task DeleteFromX(
        string postId
    )
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        _ = await _xContext.Client.DeleteTweetAsync(postId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    public async Task<Tweet?> PostToX(
        Post post,
        Embed? embed,
        string? replyId = null
    )
    {
        Tweet? sentMessage;
        List<Media>? messageAttachments = null;
        bool hasEmbedsButFailed = false;

        if (
            embed != null && embed.Type == Enums.EmbedType.Gif ||
            embed != null && embed.Type == Enums.EmbedType.Images ||
            embed != null && embed.Type == Enums.EmbedType.Video
        )
        {
            if (embed.Items.Count() > 0)
            {
                messageAttachments = new List<Media>();

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

                    if (fileByteArray == null)
                    {
                        hasEmbedsButFailed = true;
                        break;
                    }

                    Say.Info($"Uploading '{embedItem.Ref}' to X...");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var attachment = await _xContext.Client.UploadMediaAsync(
                        fileByteArray,
                        mediaCategory: mediaCategory,
                        mediaType: embedItem.MimeType
                    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    if (mediaCategory == "tweet_video")
                    {
                        do
                        {
                            if (attachment != null)
                            {
                                int checkAfterSeconds = attachment?.ProcessingInfo?.CheckAfterSeconds ?? 0;
                                await Task.Delay(checkAfterSeconds * 1000);
                            }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                            attachment =
                                await
                                (from stat in _xContext.Client.Media
                                 where stat.Type == MediaType.Status &&
                                     stat.MediaID == attachment.MediaID
                                 select stat)
                                .SingleOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        } while (attachment?.ProcessingInfo?.State == MediaProcessingInfo.InProgress);
                    }

                    if (attachment != null)
                    {
                        if (
                            mediaCategory != "tweet_video" ||
                            mediaCategory == "tweet_video" && attachment?.ProcessingInfo?.State == MediaProcessingInfo.Succeeded
                        )
                        {
                            messageAttachments.Add(attachment);
                        }
                        else
                        {
                            Say.Warning($"Failed to upload '{embedItem.Ref}' to X");
                            hasEmbedsButFailed = true;
                            break;
                        }
                    }
                }
            }
        }

        if (
            messageAttachments != null &&
            messageAttachments.Count() > 0 &&
            !hasEmbedsButFailed
        )
        {
            List<string> mediaIds = new List<string>();

            foreach (var messageAttachment in messageAttachments)
            {
                mediaIds.Add(messageAttachment.MediaID.ToString());
            }

            if (replyId != null)
                // NOTE: There's no "ReplyMediaAsync" function so we'll quote for replies
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                sentMessage = await _xContext.Client.TweetMediaAsync(
                    mediaIds: mediaIds,
                    quoteTweetID: replyId,
                    text: await GeneratePostText(post)
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            else
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                sentMessage = await _xContext.Client.TweetMediaAsync(
                    mediaIds: mediaIds,
                    text: await GeneratePostText(post)
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        else
        {
            if (String.IsNullOrEmpty(replyId))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                sentMessage = await _xContext.Client.TweetAsync(
                    text: await GeneratePostText(
                        post,
                        hasEmbedsButFailed,
                        embed != null ? embed.Type : EmbedType.Unknown
                    )
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                sentMessage = await _xContext.Client.ReplyAsync(
                    replyTweetID: replyId,
                    text: await GeneratePostText(
                        post,
                        hasEmbedsButFailed,
                        embed != null ? embed.Type : EmbedType.Unknown
                    )
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        return sentMessage;
    }

    async Task<string> GeneratePostText(
        Post post,
        bool hasEmbedsButFailed = false,
        EmbedType embedType = EmbedType.Unknown
    )
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
        captionText = RichTextUtilities.UnTruncateMarkdownLinks(captionText);

        bool forceReadMoreText = false;
        string readMoreLink = _bskyHelpers.GetPostLink(post);
        string readMoreText = "";

        if (!String.IsNullOrEmpty(captionText))
            readMoreText = $"{Environment.NewLine}—{Environment.NewLine}";

        if (hasEmbedsButFailed)
        {
            forceReadMoreText = true;
            readMoreText += embedType switch {
                EmbedType.Images => _i18nHelpers.GetPhrase(Phrase.SeeMore_Photos, post.Language, readMoreLink),
                EmbedType.Video => _i18nHelpers.GetPhrase(Phrase.SeeMore_Video, post.Language, readMoreLink),
                _ => _i18nHelpers.GetPhrase(Phrase.SeeMore_Attachment, post.Language, readMoreLink)
            };
        }
        else
            readMoreText += _i18nHelpers.GetPhrase(Phrase.SeeMore_Read, post.Language, readMoreLink);

        int captionTextLength = Encoding.UTF8.GetBytes(captionText).Length;
        int readMoreTextLength = Encoding.UTF8.GetBytes(readMoreText).Length;

        if (
            forceReadMoreText ||
            captionTextLength > XPostTextLimit
        )
            captionText = StringUtilities.Truncate(captionText, XPostTextLimit - readMoreTextLength) + readMoreText;

        return captionText;
    }

    async Task<string> ReplaceUsernames(string originalString)
    {
        string pattern = "(\\[@.*?\\]\\(https:\\/\\/bsky.app\\/profile\\/(.*?)\\))";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string href = match.Value;
            if (match.Groups[2] != null)
            {
                string did = match.Groups[2].Value;
                string? xHandle = await UsernameMaps.GetXHandleForDid(did);

                if (!String.IsNullOrEmpty(xHandle))
                {
                    originalString = originalString.Replace(
                        href,
                        $"@{xHandle}"
                    );
                }
                else
                {
                    string? bskyDisplayName = await _bskyHelpers.GetDisplayNameForDid(did);

                    if(!String.IsNullOrEmpty(bskyDisplayName))
                    {
                        // TODO: Attempt to clean name up (e.g. 🔜's)
                        originalString = originalString.Replace(
                            href,
                            bskyDisplayName
                        );
                    }
                }
            }
        }

        return originalString;
    }
}