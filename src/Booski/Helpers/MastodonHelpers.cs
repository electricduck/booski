using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Enums;
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
    private IBskyHelpers _bskyHelpers;
    private IFileCacheContext _fileCacheContext;
    private IMastodonContext _mastodonContext;

    public MastodonHelpers(
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        IMastodonContext mastodonContext
    )
    {
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
        List<Attachment> messageAttachments = null;
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

                    Say.Info($"Uploading '{embedItem.Ref}' to Mastodon...");
                    var mastodonMedia = new MediaDefinition(file, embedItem.Uri.ToString().Split('/').Last());
                    var messageAttachment = await _mastodonContext.Client.UploadMedia(mastodonMedia);

                    if(messageAttachment != null)
                    {
                        messageAttachments.Add(messageAttachment);
                    }
                    else
                    {
                        Say.Warning($"Failed to upload '{embedItem.Ref}' to Mastodon");
                        hasEmbedsButFailed = true;
                        break;
                    }
                }
            }
        }

        string[]? mediaIds = null;
        bool sensitive = false;

        if (
            messageAttachments != null &&
            messageAttachments.Count() > 0 &&
            !hasEmbedsButFailed
        )
            mediaIds = messageAttachments.Select(ma => ma.Id).ToArray();

        if(post.Sensitivity != Enums.Sensitivity.None)
            sensitive = true;

        sentMessage = await _mastodonContext.Client.PublishStatus(
            mediaIds: mediaIds,
            replyStatusId: replyId,
            sensitive: sensitive,
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

        if(hasEmbedsButFailed)
        {
            string attachmentLink = _bskyHelpers.GetPostLink(post);
            
            if(!String.IsNullOrEmpty(statusText))
                statusText += $"{Environment.NewLine}—{Environment.NewLine}";

            switch(embedType)
            {
                case EmbedType.Images:
                    statusText += $"[📷 See Photos on Bluesky]({attachmentLink})";
                    break;
                case EmbedType.Video:
                    statusText += $"[▶️ Watch Video on Bluesky]({attachmentLink})";
                    break;
                default:
                    statusText += $"[🔗 See Attachment on Bluesky]({attachmentLink})";
                    break;
            }
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
            }
        }

        return originalString;
    }
}