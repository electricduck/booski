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
    Task<List<Message>> PostToTelegram(
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
    private ITelegramContext _telegramContext;

    public TelegramHelpers(
        IBskyHelpers bskyHelpers,
        IFileCacheContext fileCacheContext,
        ITelegramContext telegramContext
    )
    {
        _bskyHelpers = bskyHelpers;
        _fileCacheContext = fileCacheContext;
        _telegramContext = telegramContext;
    }

    public async Task DeleteFromTelegram(
        long chatId,
        int messageId
    )
    {
        await _telegramContext.Client.DeleteMessageAsync(
            chatId: chatId,
            messageId: messageId
        );
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
            chatId = _telegramContext.State.Channel;

        if (
            embed != null && 
            embed.Items.Count() > 0
        )
        {
            if (embed.Type == Enums.EmbedType.Images)
            {
                var telegramAlbum = new List<IAlbumInputMedia>();
                bool firstMediaItem = true;

                foreach (var embedItem in embed.Items)
                {                    
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
                    var sentMessagesArray = await _telegramContext.Client.SendMediaGroupAsync(
                        chatId: chatId,
                        media: telegramAlbum,
                        replyParameters: telegramReply
                    );

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
                            sentMessages.Add(
                                await _telegramContext.Client.SendAnimationAsync(
                                    animation: new InputFileStream(fileStream, firstEmbedItem.GenerateFilename()),
                                    caption: await GenerateCaption(post),
                                    chatId: chatId,
                                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                    replyParameters: telegramReply
                                )
                            );
                            break;
                        case Enums.EmbedType.Video:
                            sentMessages.Add(
                                await _telegramContext.Client.SendVideoAsync(
                                    caption: await GenerateCaption(post),
                                    chatId: chatId,
                                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                                    replyParameters: telegramReply,
                                    video: new InputFileStream(fileStream, firstEmbedItem.GenerateFilename())
                                )
                            );
                            break;
                    }
                }
            }
            else
            {
                string text = $"{await GenerateCaption(post)}<a href=\"{embed.Items.First().Uri}\"> </a>";

                sentMessages.Add(
                    await _telegramContext.Client.SendTextMessageAsync(
                        chatId: chatId,
                        linkPreviewOptions: telegramLinkPreviewDisabled,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyParameters: telegramReply,
                        text: text
                    )
                );
            }
        }
        
        if(
            embed == null ||
            embed != null && embed.Items.Count() == 0 ||
            embed != null && embed.Items.Count() > 0 && hasEmbedsButFailed
        )
        {
            sentMessages.Add(
                await _telegramContext.Client.SendTextMessageAsync(
                    chatId: chatId,
                    linkPreviewOptions: telegramLinkPreviewDisabled,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyParameters: telegramReply,
                    text: await GenerateCaption(post, hasEmbedsButFailed, (embed != null) ? embed.Type : EmbedType.Unknown)
                )
            );
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

        string captionText = $"""
<a href="https://bsky.app/profile/{post.Profile.Did}">🦋 @{post.Profile.Handle}</a>
""";

        if(hasEmbedsButFailed)
        {
            string attachmentLink = _bskyHelpers.GetPostLink(post);
            
            captionText = $"{Environment.NewLine}—{Environment.NewLine}{captionText}";

            switch(embedType)
            {
                case EmbedType.Images:
                    captionText = $"<a href=\"{attachmentLink}\">📷 See Photos on Bluesky</a>{captionText}";
                    break;
                case EmbedType.Video:
                    captionText = $"<a href=\"{attachmentLink}\">▶️ Watch Video on Bluesky</a>{captionText}";
                    break;
                default:
                    captionText = $"<a href=\"{attachmentLink}\">🔗 See Attachment on Bluesky</a>{captionText}";
                    break;
            }
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

    void SayUploadMessage(string embedRef)
    {
        Say.Info($"Uploading '{embedRef}' to Telegram...");
    }
}