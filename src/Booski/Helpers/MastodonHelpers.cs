using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Enums;
using Booski.Utilities;
using Mastonet;
using Mastonet.Entities;

namespace Booski.Helpers;

public interface IMastodonHelpers
{
    Task DeleteFromMastodon(string statusId);
    Task<Status?> PostToMastodon(
        Post post,
        Embed? embed,
        string? replyId = null
    );
}

internal sealed class MastodonHelpers : IMastodonHelpers
{
    private IBridgyFedHelpers _bridgyFedHelpers;
    private IBskyHelpers _bskyHelpers;
    private IFileCacheContext _fileCacheContext;
    private IMastodonContext _mastodonContext;

    public MastodonHelpers(
        IBridgyFedHelpers bridgyFedHelpers,
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        IMastodonContext mastodonContext
    )
    {
        _bridgyFedHelpers = bridgyFedHelpers;
        _bskyHelpers = bskyHelpers;
        _fileCacheContext = fileCacheContext;
        _mastodonContext = mastodonContext;
    }

    public async Task DeleteFromMastodon(
        string statusId
    )
    {
        await _mastodonContext.Client.DeleteStatus(statusId);
    }

    public async Task<Status?> PostToMastodon(
        Post post,
        Embed? embed,
        string? replyId = null
    )
    {
        Status? sentMessage = null;
        List<Attachment>? messageAttachments = null;
        bool hasEmbedsButFailed = false;

        if (
            embed != null && embed.Type == Enums.EmbedType.Gif ||
            embed != null && embed.Type == Enums.EmbedType.Images ||
            embed != null && embed.Type == Enums.EmbedType.Video
        )
        {
            if (embed.Items.Count() > 0)
            {
                messageAttachments = new List<Attachment>();

                foreach (var embedItem in embed.Items)
                {
                    var file = await _fileCacheContext.GetFileFromUri(embedItem.Uri);

                    if(file == null)
                    {
                        hasEmbedsButFailed = true;
                        break;
                    }

                    Say.Info($"Uploading '{embedItem.Ref}' to {_mastodonContext.State.InstanceSoftware}...");
                    var mastodonMedia = new MediaDefinition(file, embedItem.Uri.ToString().Split('/').Last());
                    var messageAttachment = await _mastodonContext.Client.UploadMedia(mastodonMedia);

                    if(messageAttachment != null)
                    {
                        messageAttachments.Add(messageAttachment);
                    }
                    else
                    {
                        Say.Warning($"Failed to upload '{embedItem.Ref}' to {_mastodonContext.State.InstanceSoftware}");
                        hasEmbedsButFailed = true;
                        break;
                    }
                }
            }
        }

        string[]? mediaIds = null;
        bool sensitive = false;
        string? spoiler = "";

        if (
            messageAttachments != null &&
            messageAttachments.Count() > 0 &&
            !hasEmbedsButFailed
        )
            mediaIds = messageAttachments.Select(ma => ma.Id).ToArray();

        if(post.Sensitivity != Enums.Sensitivity.None)
        {
            sensitive = true;
            spoiler = post.Sensitivity switch
            {
                Sensitivity.Suggestive => "🔞 Suggestive",
                Sensitivity.Nudity => "🔞 Nudity",
                Sensitivity.Porn => "🔞 Porn",
                _ => String.Empty
            };
        }

        sentMessage = await _mastodonContext.Client.PublishStatus(
            mediaIds: mediaIds,
            replyStatusId: replyId,
            sensitive: sensitive,
            spoilerText: spoiler,
            status: await GenerateStatusText(
                post,
                hasEmbedsButFailed,
                embed != null ? embed.Type : EmbedType.Unknown
            ),
            visibility: Visibility.Public
        );

        return sentMessage;
    }

    async Task<string> GenerateStatusText(
        Post post,
        bool hasEmbedsButFailed = false,
        EmbedType embedType = EmbedType.Unknown
    )
    {
        string statusText = _bskyHelpers.ParseFacets(
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

        if(_mastodonContext.State.NoRichText)
            statusText = RichTextUtilities.UnTruncateMarkdownLinks(statusText);

        if(hasEmbedsButFailed)
        {
            string attachmentLink = _bskyHelpers.GetPostLink(post);
            
            if(!String.IsNullOrEmpty(statusText))
                statusText += $"{Environment.NewLine}—{Environment.NewLine}";

            if(_mastodonContext.State.NoRichText)
                statusText += embedType switch {
                    EmbedType.Images => $"📷 See Photos: {attachmentLink}",
                    EmbedType.Video => $"▶️ Watch Video: {attachmentLink}",
                    _ => $"🔗 See Attachment: {attachmentLink}"
                };
            else
                statusText += embedType switch {
                    EmbedType.Images => $"[📷 See Photos on Bluesky]({attachmentLink})",
                    EmbedType.Video => $"[▶️ Watch Video on Bluesky]({attachmentLink})",
                    _ => $"[🔗 See Attachment on Bluesky]({attachmentLink})"
                };
        }

        return statusText;
    }
    async Task<string> ReplaceUsernames(string originalString)
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
                else
                {
                    var bskyBridgyHandle = await _bridgyFedHelpers.GetBridgyBskyHandle(did);

                    if(!String.IsNullOrEmpty(bskyBridgyHandle))
                    {
                        originalString = originalString.Replace(
                            href,
                            $"@{bskyBridgyHandle}"
                        );
                    }
                }
            }
        }

        return originalString;
    }
}