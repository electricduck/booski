using System.Text.RegularExpressions;
using Booski.Common;
using Booski.Contexts;
using Booski.Data;
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
    private ITelegramContext _telegramContext;

    public TelegramHelpers(
        IBskyHelpers bskyHelpers,
        ITelegramContext telegramContext
    )
    {
        _bskyHelpers = bskyHelpers;
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

    public async Task<List<Message>> PostToTelegram(
        Post post,
        Embed? embed,
        string? chatId = null,
        int? replyId = null
    )
    {
        List<Message> sentMessages = new List<Message>();

        if(chatId == null)
            chatId = _telegramContext.State.Channel;

        if (embed != null && embed.Items != null && embed.Items.Count() > 0)
        {
            if (embed.Type == Enums.EmbedType.Images)
            {
                var telegramAlbum = new List<IAlbumInputMedia>();
                bool firstMediaItem = true;

                foreach (var embedItem in embed.Items)
                {
                    var telegramMediaPhoto = new InputMediaPhoto(new InputFileUrl(embedItem.Uri));

                    if (firstMediaItem)
                    {
                        telegramMediaPhoto.Caption = await GenerateCaption(post);
                        telegramMediaPhoto.ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;

                        firstMediaItem = false;
                    }

                    telegramAlbum.Add(telegramMediaPhoto);
                }

                var sentMessagesArray = await _telegramContext.Client.SendMediaGroupAsync(
                    chatId: chatId,
                    media: telegramAlbum,
                    replyToMessageId: replyId
                );

                sentMessages = sentMessagesArray.ToList();
            }
            else if (embed.Type == Enums.EmbedType.Gif)
            {
                sentMessages.Add(
                    await _telegramContext.Client.SendAnimationAsync(
                        animation: new InputFileUrl(embed.Items.First().Uri),
                        caption: await GenerateCaption(post),
                        chatId: chatId,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyToMessageId: replyId
                    )
                );
            }
            else
            {
                string text = $"{await GenerateCaption(post)}<a href=\"{embed.Items.First().Uri}\"> </a>";

                sentMessages.Add(
                    await _telegramContext.Client.SendTextMessageAsync(
                        chatId: chatId,
                        disableWebPagePreview: false,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyToMessageId: replyId,
                        text: text
                    )
                );
            }
        }
        else
        {
            sentMessages.Add(
                await _telegramContext.Client.SendTextMessageAsync(
                    chatId: chatId,
                    disableWebPagePreview: true,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyToMessageId: replyId,
                    text: await GenerateCaption(post)
                )
            );
        }

        return sentMessages;
    }

    async Task<string> GenerateCaption(Post post)
    {
        string originalCaptionText = _bskyHelpers.ParseFacets(
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

    async Task<string> ReplaceUsernames(string originalString)
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