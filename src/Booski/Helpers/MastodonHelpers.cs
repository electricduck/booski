using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
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
        List<Attachment> messageAttachments = new List<Attachment>();

        if (
            embed != null && embed.Type == Enums.EmbedType.Gif ||
            embed != null && embed.Type == Enums.EmbedType.Images ||
            embed != null && embed.Type == Enums.EmbedType.Video
        )
        {
            if (embed.Items.Count() > 0)
            {
                foreach (var embedItem in embed.Items)
                {
                    var file = await _fileCacheContext.GetFileFromUri(embedItem.Uri);

                    if(file == null)
                        return null;

                    var mastodonMedia = new MediaDefinition(file, embedItem.Uri.ToString().Split('/').Last());
                    var messageAttachment = await _mastodonContext.Client.UploadMedia(mastodonMedia);

                    messageAttachments.Add(messageAttachment);
                }
            }
        }

        bool sensitive = false;

        if(post.Sensitivity != Enums.Sensitivity.None)
            sensitive = true;

        /*sentMessage = await _mastodonContext.Client.PublishStatus(
            mediaIds: messageAttachments.Select(ma => ma.Id).ToArray(),
            replyStatusId: replyId,
            sensitive: sensitive,
            status: await GenerateStatusText(post),
            visibility: Visibility.Public
        );*/

        return sentMessage;
    }

    async Task<string> GenerateStatusText(Post post)
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