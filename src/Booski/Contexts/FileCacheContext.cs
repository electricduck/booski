
namespace Booski.Contexts;

// TODO: Actually cache stuff
public interface IFileCacheContext
{
    Task<Stream> GetFileFromUri(Uri uri);
    Task<byte[]> GetFileFromUriAsByteArray(Uri uri);
    Task<MemoryStream> GetFileFromUriAsMemoryStream(Uri uri);
}

internal sealed class FileCacheContext : IFileCacheContext
{
    private IHttpContext _httpContext;

    public FileCacheContext(
        IHttpContext httpContext
    )
    {
        _httpContext = httpContext;
    }

    public async Task<Stream> GetFileFromUri(Uri uri)
    {
        if(_httpContext.Client == null)
            _httpContext.CreateClient();

        var fileStream = await _httpContext.Client.GetStreamAsync(uri);

        return fileStream;
    }

    public async Task<byte[]> GetFileFromUriAsByteArray(Uri uri)
    {
        var memoryStream = await GetFileFromUriAsMemoryStream(uri);
        var byteArray = memoryStream.ToArray();

        return byteArray;
    }

    public async Task<MemoryStream> GetFileFromUriAsMemoryStream(Uri uri)
    {
        var fileStream = await GetFileFromUri(uri);

        MemoryStream memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);

        return memoryStream;
    }
}