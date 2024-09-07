namespace Booski.Clients;
using Mastonet;
using Booski.Common;

public class MastodonClient
{
    public static async Task DeleteFromMastodon(
        Mastonet.MastodonClient client,
        string statusId
    )
    {
        await client.DeleteStatus(statusId);
    }

    public static async Task<Mastonet.Entities.Status> PostToMastodon(
        Mastonet.MastodonClient client,
        Embed? embed,
        string text,
        string? reply = null,
        bool sensitive = false
    )
    {
        Mastonet.Entities.Status sentMessage = null;
        List<Mastonet.Entities.Attachment> messageAttachments = new List<Mastonet.Entities.Attachment>();

        if (
            embed.Type == Enums.EmbedType.Gif ||
            embed.Type == Enums.EmbedType.Images
        )
        {
            if (embed.Items.Count() > 0)
            {
                var httpClient = new HttpClient();

                foreach (var embedItem in embed.Items)
                {
                    var file = await FileCache.GetFileFromUri(embedItem.Uri);

                    var mastodonMedia = new MediaDefinition(file, embedItem.Uri.ToString().Split('/').Last());
                    var messageAttachment = await client.UploadMedia(mastodonMedia);

                    messageAttachments.Add(messageAttachment);
                }
            }
        }

        sentMessage = await client.PublishStatus(
            mediaIds: messageAttachments.Select(ma => ma.Id).ToArray(),
            replyStatusId: reply,
            sensitive: sensitive,
            status: text,
            visibility: Visibility.Public
        );

        return sentMessage;
    }
}