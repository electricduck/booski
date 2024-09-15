using Booski.Data;

namespace Booski.Contexts;

// Is this a Context or a Helper?
public interface IFileCacheContext
{
    Task ClearCache();
    Task<Stream?> GetFileFromUri(Uri uri);
    Task<byte[]?> GetFileFromUriAsByteArray(Uri uri);
    Task<MemoryStream?> GetFileFromUriAsMemoryStream(Uri uri);
}

internal sealed class FileCacheContext : IFileCacheContext
{
    private IHttpContext _httpContext;
    private IYtDlpContext _ytDlpContext;

    public FileCacheContext(
        IHttpContext httpContext,
        IYtDlpContext ytDlpContext
    )
    {
        _httpContext = httpContext;
        _ytDlpContext = ytDlpContext;
    }

    public async Task ClearCache()
    {
        Say.Debug("[FileCacheContext.ClearCache] Clearing cache...");

        var availableFileCaches = await FileCaches.GetFileCaches();

        foreach(var availableFileCache in availableFileCaches)
        {
            var downloadedAt = availableFileCache.DownloadedAt;
            var filePath = GetFilePath(availableFileCache.Filename);

            if(
                availableFileCache.Available &&
                downloadedAt != null &&
                DateTime.UtcNow > downloadedAt.Value.Date.AddHours(24) &&
                File.Exists(filePath)
            )
            {
                Console.WriteLine($"Deleting '{filePath}'");

                File.Delete(filePath);
                await FileCaches.DeleteFileCache(availableFileCache.Uri);
            }
        }
    }

    public async Task<Stream?> GetFileFromUri(Uri uri)
    {
        Stream? fileStream = null;
        var filePath = await GetFileFromCache(uri);

        if(filePath != null)
            fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        return fileStream;
    }

    public async Task<byte[]?> GetFileFromUriAsByteArray(Uri uri)
    {
        var memoryStream = await GetFileFromUriAsMemoryStream(uri);

        if(memoryStream != null)
        {
            var byteArray = memoryStream.ToArray();
            return byteArray;
        }
        else
        {
            return null;
        }
    }

    public async Task<MemoryStream?> GetFileFromUriAsMemoryStream(Uri uri)
    {
        var fileStream = await GetFileFromUri(uri);

        if(fileStream != null)
        {
            MemoryStream memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            return memoryStream;
        }
        else
        {
            return null;
        }
    }

    async Task<bool> DownloadFile(Uri uri, string filePath)
    {
        if(_httpContext.Client == null)
            _httpContext.CreateClient();

        if(uri.ToString().EndsWith(".m3u8"))
        {
            Say.Info($"Grabbing '{uri}'...");
            _ytDlpContext.DownloadVideo(uri, filePath);
        }
        else
        {
            Say.Info($"Downloading '{uri}'...");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            using (var stream = await _httpContext.Client.GetStreamAsync(uri))
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
                    await stream.CopyToAsync(fs);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        if(File.Exists(filePath))
            return true;
        else
            return false;
    }

    async Task<string?> GetFileFromCache(Uri uri)
    {
        var foundFileCache = await FileCaches.GetFileCache(uri.ToString());
        string filePath;

        if(
            foundFileCache != null &&
            foundFileCache.Available &&
            File.Exists(GetFilePath(foundFileCache.Filename))
        )
        {
            filePath = GetFilePath(foundFileCache.Filename);
        }
        else
        {
            var filename = Guid.NewGuid().ToString();
            filePath = GetFilePath(filename);

            var isFileDownloaded = await DownloadFile(uri, filePath);

            if(isFileDownloaded)
            {
                var fileSize = new FileInfo(filePath).Length;
                var newFileCache = await FileCaches.AddOrUpdateFileCache(
                    filename: filename,
                    fileSize: fileSize,
                    uri: uri.ToString()
                );
            }
        }

        if(
            !String.IsNullOrEmpty(filePath) &&
            File.Exists(filePath)
        )
            return filePath;
        else
        {
            Say.Warning($"Unable to fetch '{uri}'");
            await FileCaches.DeleteFileCache(uri.ToString());
            return null;
        }
    }

    string GetFilePath(string filename)
    {
        string filePath = "";

        if(!String.IsNullOrEmpty(Program.FileCacheDir))
            return Path.Combine(Program.FileCacheDir, filename);
        
        return filePath;
    }
}