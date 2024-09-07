namespace Booski.Clients;
using Telegram.Bot;
using Telegram.Bot.Types;
using Booski.Common;

public class TelegramClient
{
    public static async Task DeleteFromTelegram(
        TelegramBotClient client,
        long telegramChatId,
        int telegramMessageId
    )
    {
        await client.DeleteMessageAsync(
            chatId: telegramChatId,
            messageId: telegramMessageId
        );
    }

    public static async Task<List<Telegram.Bot.Types.Message>> PostToTelegram(
        TelegramBotClient client,
        string chatId,
        Embed? embed,
        string text,
        int? reply = null
    )
    {
        List<Telegram.Bot.Types.Message> sentMessages = new List<Telegram.Bot.Types.Message>();

        if (embed.Items != null && embed.Items.Count() > 0)
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
                        telegramMediaPhoto.Caption = text;
                        telegramMediaPhoto.ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;

                        firstMediaItem = false;
                    }

                    telegramAlbum.Add(telegramMediaPhoto);
                }

                var sentMessagesArray = await client.SendMediaGroupAsync(
                    chatId: chatId,
                    media: telegramAlbum,
                    replyToMessageId: reply
                );

                sentMessages = sentMessagesArray.ToList();
            }
            else if (embed.Type == Enums.EmbedType.Gif)
            {
                sentMessages.Add(
                    await client.SendAnimationAsync(
                        animation: new InputFileUrl(embed.Items.First().Uri),
                        caption: text,
                        chatId: chatId,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyToMessageId: reply
                    )
                );
            }
            else
            {
                text = $"{text}<a href=\"{embed.Items.First().Uri}\"> </a>";

                sentMessages.Add(
                    await client.SendTextMessageAsync(
                        chatId: chatId,
                        disableWebPagePreview: false,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                        replyToMessageId: reply,
                        text: text
                    )
                );
            }
        }
        else
        {
            sentMessages.Add(
                await client.SendTextMessageAsync(
                    chatId: chatId,
                    disableWebPagePreview: true,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                    replyToMessageId: reply,
                    text: text
                )
            );
        }

        return sentMessages;
    }
}