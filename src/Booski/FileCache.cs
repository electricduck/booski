namespace Booski;

// TODO: Actually cache stuff
public class FileCache
{
    public static async Task<Stream> GetFileFromUri(Uri uri)
    {
        var httpClient = new HttpClient();
        var fileStream = await httpClient.GetStreamAsync(uri);

        return fileStream;
    }

    public static async Task<byte[]> GetFileFromUriAsByteArray(Uri uri)
    {
        var memoryStream = await GetFileFromUriAsMemoryStream(uri);
        var byteArray = memoryStream.ToArray();

        return byteArray;
    }

    public static async Task<MemoryStream> GetFileFromUriAsMemoryStream(Uri uri)
    {
        var fileStream = await GetFileFromUri(uri);

        MemoryStream memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);

        return memoryStream;
    }
}