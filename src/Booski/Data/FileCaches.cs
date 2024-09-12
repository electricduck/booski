using Microsoft.EntityFrameworkCore;
using Booski.Common;

namespace Booski.Data;

public class FileCaches
{
    public async static Task<FileCache> AddOrUpdateFileCache(
        string uri,
        string filename,
        long fileSize
    )
    {
        using (var db = new Database())
        {
            FileCache fileCache = new FileCache();

            var existingFileCache = await db
                .FileCaches
                .Where(fc => fc.Uri == uri)
                .FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            if (existingFileCache == null)
            {
                fileCache = new FileCache
                {
                    Available = true,
                    DownloadedAt = now,
                    Filename = filename,
                    FileSize = fileSize,
                    Uri = uri
                };

                await db.FileCaches.AddAsync(fileCache);
                await db.SaveChangesAsync();
            }
            else
            {
                existingFileCache.Available = true;
                existingFileCache.DownloadedAt = now;
                existingFileCache.Filename = filename;
                existingFileCache.FileSize = fileSize;

                await db.SaveChangesAsync();
            }

            return fileCache;
        }
    }

    public async static Task<FileCache?> DeleteFileCache(
        string uri
    )
    {
        using (var db = new Database())
        {
            var fileCache = await db
                .FileCaches
                .Where(fc => fc.Uri == uri)
                .FirstOrDefaultAsync();

            if(fileCache != null)
            {
                fileCache.Available = false;
                fileCache.DownloadedAt = null;
                fileCache.FileSize = null;
            }

            await db.SaveChangesAsync();

            return fileCache;
        }
    }

    public async static Task<FileCache?> GetFileCache(
        string uri
    )
    {
        using (var db = new Database())
        {
            var fileCache = await db
                .FileCaches
                .Where(fc => fc.Available == true)
                .Where(fc => fc.Uri == uri)
                .FirstOrDefaultAsync();

            return fileCache;
        }
    }

    public async static Task<List<FileCache>> GetFileCaches()
    {
        using (var db = new Database())
        {
            var fileCaches = await db
                .FileCaches
                .Where(fc => fc.Available == true)
                .ToListAsync();

            return fileCaches;
        }
    }
}