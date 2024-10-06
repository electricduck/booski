using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Booski.Helpers;

public interface ITelegramHelpers
{
    Task DeleteFromTelegram(
        long chatId,
        int messageId
    );
    Task<List<Message>?> PostToTelegram(
        Post post,
        Embed? embed,
        string? chatId = null,
        int? replyId = null
    );
}

internal sealed class TelegramHelpers : ITelegramHelpers
{
    private IBskyHelpers _bskyHelpers;
    private IFileCacheContext _fileCacheContext;
    private II18nHelpers _i18nHelpers;
    private IMastodonContext _mastodonContext;
    private ITelegramContext _telegramContext;
    private IXContext _xContext;

    public TelegramHelpers(
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        II18nHelpers i18nHelpers,
        IMastodonContext mastodonContext,
        ITelegramContext telegramContext,
        IXContext xContext
    )
    {
        _bskyHelpers = bskyHelpers;
        _fileCacheContext = fileCacheContext;
        _i18nHelpers = i18nHelpers;
        _mastodonContext = mastodonContext;
        _telegramContext = telegramContext;
        _xContext = xContext;
    }

    public async Task DeleteFromTelegram(
        long chatId,
        int messageId
    )
    {
#pragma warning disable CS8604 // Possible null reference argument.
        await _telegramContext.Client.DeleteMessageAsync(
            chatId: chatId,
            messageId: messageId
        );
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public async Task<List<Message>?> PostToTelegram(
        Post post,
        Embed? embed,
        string? chatId = null,
        int? replyId = null
    )
    {
        List<Message> sentMessages = new List<Message>();
        bool hasEmbedsButFailed = false;
        ReplyParameters telegramReply = new ReplyParameters();
        LinkPreviewOptions telegramLinkPreviewDisabled = new LinkPreviewOptions
        {
            IsDisabled = true
        };

        if(replyId != null)
            telegramReply.MessageId = (int)replyId;

        if (chatId == null)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            chatId = _telegramContext.State.Channel;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (
            embed != null &&
            embed.Items != null &&
            embed.Items.Count() > 0
        )
        {
            if (embed.Type == Enums.EmbedType.Images)
            {
                var telegramAlbum = new List<IAlbumInputMedia>();
                bool firstMediaItem = true;

                foreach (var embedItem in embed.Items)
                {
                    if(embedItem.Ref == null || embedItem.Uri == null)
                        break;

                    var fileStream = await _fileCacheContext.GetFileFromUri(embedItem.Uri);

                    if (fileStream == null)
                    {
                        hasEmbedsButFailed = true;
                        break;
                    }

                    var telegramMediaPhoto = new InputMediaPhoto(new InputFileStream(fileStream, embedItem.GenerateFilename()));

                    if (firstMediaItem)
                    {
                        telegramMediaPhoto.Caption = await GenerateCaption(post);
                        telegramMediaPhoto.ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;

                        firstMediaItem = false;
                    }

                    SayUploadMessage(embedItem.Ref);
                    telegramAlbum.Add(telegramMediaPhoto);
                }

                if(telegramAlbum.Count() > 0 && !hasEmbedsButFailed)
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    var sentMessagesArray = await _telegramContext.Client.SendMediaGroupAsync(
                        chatId: chatId,
                        media: telegramAlbum,
                        replyParameters: telegramReply
                    );
#pragma warning restore CS8604 // Possible null reference argument.

                    sentMessages = sentMessagesArray.ToList();
                }
            }
            else if (
                embed.Type == Enums.EmbedType.Gif ||
                embed.Type == Enums.EmbedType.Video
            )
            {
                var firstEmbedItem = embed.Items.First();
                    
                var fileStream = await _fileCacheContext.GetFileFromUri(firstEmbedItem.Uri);

                if (fileStream == null)
                    hasEmbedsButFailed = true;
                else
                {
                    SayUploadMessage(firstEmbedItem.Ref);

                    switch(embed.Type)
                    {
                        case Enums.EmbedType.Gif:
#pragma warning disable CS8604 // Possible null reference argument.
                            sentMessages.Add(
                                await _telegramContext.Client.SendAnimationAsync(
                                    animation: new InputFileStream(fileStream, firstEmbedItem.GenerateFilename()),
                                    caption: await GenerateCaption(post),
                                    chatId: chatId,
                                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                    replyParameters: telegramReply
                                )
                            );
#pragma warning restore CS8604 // Possible null reference argument.
                            break;
                        case Enums.EmbedType.Video:
#pragma warning disable CS8604 // Possible null reference argument.
                            sentMessages.Add(
                                await _telegramContext.Client.SendVideoAsync(
                                    caption: await GenerateCaption(post),
                                    chatId: chatId,
                                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                    replyParameters: telegramReply,
                                    video: new InputFileStream(fileStream, firstEmbedItem.GenerateFilename())
                                )
                            );
#pragma warning restore CS8604 // Possible null reference argument.
                            break;
                    }
                }
            }
            else
            {
                string text = $"{await GenerateCaption(post)}<a href=\"{embed.Items.First().Uri}\"> </a>";

#pragma warning disable CS8604 // Possible null reference argument.
                sentMessages.Add(
                    await _telegramContext.Client.SendTextMessageAsync(
                        chatId: chatId,
                        linkPreviewOptions: telegramLinkPreviewDisabled,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyParameters: telegramReply,
                        text: text
                    )
                );
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }
        
        if(
            embed == null ||
            embed != null && embed.Items != null && embed.Items.Count() == 0 ||
            embed != null && embed.Items != null && embed.Items.Count() > 0 && hasEmbedsButFailed
        )
        {
#pragma warning disable CS8604 // Possible null reference argument.
            sentMessages.Add(
                await _telegramContext.Client.SendTextMessageAsync(
                    chatId: chatId,
                    linkPreviewOptions: telegramLinkPreviewDisabled,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyParameters: telegramReply,
                    text: await GenerateCaption(post, hasEmbedsButFailed, (embed != null) ? embed.Type : EmbedType.Unknown)
                )
            );
#pragma warning restore CS8604 // Possible null reference argument.
        }

        return sentMessages;
    }

    async Task<string> GenerateCaption(
        Post post,
        bool hasEmbedsButFailed = false,
        EmbedType embedType = EmbedType.Unknown
    )
    {
        string originalCaptionText = _bskyHelpers.ParseFacets(
            post.Record.Text,
            post.Record.Facets
        );
        originalCaptionText = await ReplaceUsernames(originalCaptionText);

        PostLog? fetchedPostLog = null;

        string captionText = $"""
<a href="https://bsky.app/profile/{post.Profile.Did}/post/{post.RecordKey}">🦋 @{post.Profile.Handle}</a>
""";

        if(
            _mastodonContext.IsConnected &&
            _mastodonContext.State != null
        )
        {
            if(fetchedPostLog == null)
                fetchedPostLog = await PostLogs.GetPostLogByRecordKey(post.RecordKey, post.Profile.Did);

            if(
                fetchedPostLog != null &&
                !String.IsNullOrEmpty(fetchedPostLog.Mastodon_InstanceDomain) &&
                !String.IsNullOrEmpty(fetchedPostLog.Mastodon_StatusId) &&
                _mastodonContext.State.Instance.Domain == fetchedPostLog.Mastodon_InstanceDomain
            )
            {
                string mastodonUrl = _mastodonContext.State.InstanceSoftware switch {
                    MastodonSoftware.GoToSocial =>
                        $"https://{_mastodonContext.State.Instance.Domain}/@{_mastodonContext.State.Account.UserName}/statuses/{fetchedPostLog.Mastodon_StatusId}",
                    MastodonSoftware.Pixelfed =>
                        $"https://{_mastodonContext.State.Instance.Domain}/p/{_mastodonContext.State.Account.UserName}/{fetchedPostLog.Mastodon_StatusId}",
                    MastodonSoftware.Compatible => String.Empty, // NOTE: We cannot guarantee support
                    _ =>
                        $"https://{_mastodonContext.State.Instance.Domain}/@{_mastodonContext.State.Account.UserName}/{fetchedPostLog.Mastodon_StatusId}"
                };

                if(!String.IsNullOrEmpty(mastodonUrl))
                {
                    captionText += $" | <a href=\"{mastodonUrl}\">🐘 {_mastodonContext.State.Username}</a>";
                }
            }
        }

        if(
            _xContext.IsConnected &&
            _xContext.State != null
        )
        {
            if(fetchedPostLog == null)
                fetchedPostLog = await PostLogs.GetPostLogByRecordKey(post.RecordKey, post.Profile.Did);

            if(
                fetchedPostLog != null &&
                !String.IsNullOrEmpty(fetchedPostLog.X_PostId)
            )
                captionText += $" | <a href=\"https://x.com/{_xContext.State.Username}/status/{fetchedPostLog.X_PostId}\">🐦 {_xContext.State.Username}</a>";
        }

        if(hasEmbedsButFailed)
        {
            string attachmentLink = _bskyHelpers.GetPostLink(post);
            
            captionText = $"{Environment.NewLine}—{Environment.NewLine}{captionText}";

            captionText = embedType switch {
                EmbedType.Images => $"<a href=\"{attachmentLink}\">{_i18nHelpers.GetPhrase(Phrase.SeeMoreRich_Photos, post.Language)}</a>{captionText}",
                EmbedType.Video => $"<a href=\"{attachmentLink}\">{_i18nHelpers.GetPhrase(Phrase.SeeMoreRich_Video, post.Language)}</a>{captionText}",
                _ => $"<a href=\"{attachmentLink}\">{_i18nHelpers.GetPhrase(Phrase.SeeMoreRich_Attachment, post.Language)}</a>{captionText}",
            };
        }

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

    async Task<string> ReplaceUsernames(string originalString)
    {
        string pattern = "(<a href=\"https:\\/\\/bsky.app\\/profile\\/(.*?)\">@.*?<\\/a>)";
        foreach (Match match in Regex.Matches(originalString, pattern, RegexOptions.IgnoreCase))
        {
            string href = match.Value;
            if (match.Groups[2] != null)
            {
                string did = match.Groups[2].Value;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string telegramHandle = await UsernameMaps.GetTelegramHandleForDid(did);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

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

    void SayUploadMessage(string embedRef)
    {
        Say.Info($"Uploading '{embedRef}' to Telegram...");
    }
}