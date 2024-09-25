using Booski.Common;

namespace Booski.Contexts;

public interface IHttpContext {
    HttpClient? Client { get; set; }

    void CreateClient(string userAgent = "Booski/?");
}

internal sealed class HttpContext : IHttpContext
{
    public HttpClient? Client { get; set; }

    public void CreateClient(
        string userAgent = "Booski/?"
    )
    {
        Client = new HttpClient();
        Client.DefaultRequestHeaders.Add("User-Agent", $"{userAgent}");
    }
}