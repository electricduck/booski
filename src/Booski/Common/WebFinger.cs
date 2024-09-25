namespace Booski.Common;

public class WebFinger
{
    public List<string>? Aliases { get; set; }
    public List<WebFingerLink>? Links { get; set; }
    public string? Subject { get; set; }
}