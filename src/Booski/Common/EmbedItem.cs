namespace Booski.Common;

public class EmbedItem
{
    public string MimeType { get; set; }
    public string Ref { get; set; }
    public Uri Uri { get; set; }

    // Telegram needs this. It's shit but it works
    public string GenerateFilename()
    {
        var filename = Guid.NewGuid();
        var extension = MimeType.Split("/").Last();
        return $"{filename}.{extension}";
    }
}