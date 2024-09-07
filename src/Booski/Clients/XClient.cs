namespace Booski.Clients;

using Booski.Common;
using XLib = LinqToTwitter;

public class XClient
{
    public static async Task DeleteFromX(
        XLib.TwitterContext client,
        string postId
    ) {
        await client.DeleteTweetAsync(postId);
    }
    
    public static async Task<XLib.Tweet> PostToX(
        XLib.TwitterContext client,
        Embed? embed,
        string text,
        string? reply = null
    )
    {
        XLib.Tweet sentMessage = null;
        List<XLib.Media> messageAttachments = new List<XLib.Media>();

        if (
            embed.Type == Enums.EmbedType.Gif ||
            embed.Type == Enums.EmbedType.Images
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
                    var fileByteArray = await FileCache.GetFileFromUriAsByteArray(embedItem.Uri);

                    var attachment = await client.UploadMediaAsync(
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
                sentMessage = await client.TweetMediaAsync(
                    mediaIds: mediaIds,
                    quoteTweetID: reply,
                    text: text
                );
            }
        }
        else
        {
            if (String.IsNullOrEmpty(reply))
            {
                sentMessage = await client.TweetAsync(
                    text: text
                );
            }
            else
            {
                sentMessage = await client.ReplyAsync(
                    replyTweetID: reply,
                    text: text
                );
            }
        }

        return sentMessage;
    }
}