using Booski.Common;
using Microsoft.EntityFrameworkCore;

namespace Booski.Data;

public class PostLogs
{
    public async static Task<PostLog> AddPostLog(
        string recordKey,
        string repository,
        bool ignored = false
    )
    {
        using (var db = new Database())
        {
            PostLog postLog = new PostLog();

            var existingPostLog = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            if (existingPostLog == null)
            {
                postLog = new PostLog
                {
                    Ignored = ignored,
                    RecordKey = recordKey,
                    Repository = repository,
                    UpdatedAt = DateTime.Now,
                    Version = 1
                };

                await db.PostLogs.AddAsync(postLog);
                await db.SaveChangesAsync();
            }
            else
            {
                postLog = existingPostLog;
            }

            return postLog;
        }
    }

    public async static Task<int> CountPostLogs(
        string repository,
        bool includeDeleted = false,
        bool includeIgnored = true
    )
    {
        using (var db = new Database())
        {
            var postLogsCount = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.Deleted == includeDeleted)
                .Where(pl => pl.Ignored == includeIgnored)
                .CountAsync();

            return postLogsCount;
        }
    }

    public async static Task<PostLog?> DeletePostLog(
        string recordKey,
        string repository
    )
    {
        using (var db = new Database())
        {
            var existingPostLog = await db
                .PostLogs
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            if (existingPostLog != null)
            {
                existingPostLog.Deleted = true;
                await db.SaveChangesAsync();
            }

            return existingPostLog;
        }
    }

    public async static Task<PostLog> GetPostLogByRecordKey(
        string recordKey,
        string repository
    )
    {
        using (var db = new Database())
        {
            var postLog = await db
                .PostLogs
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            return postLog;
        }
    }

    public async static Task<PostLog> GetPostLog(
        string recordKey,
        string repository
    )
    {
        using (var db = new Database())
        {
            var postLog = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            return postLog;
        }
    }

    public async static Task<List<PostLog>> GetPostLogs(
        string repository
    )
    {
        using (var db = new Database())
        {
            var postLogs = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.Deleted == false)
                .ToListAsync();

            return postLogs;
        }
    }

    public async static Task<PostLog?> IgnorePostLog(
        string recordKey,
        string repository
    )
    {
        using (var db = new Database())
        {
            var existingPostLog = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            if (existingPostLog != null)
            {
                existingPostLog.Ignored = true;
                await db.SaveChangesAsync();
            }

            return existingPostLog;
        }
    }

    public async static Task<PostLog> UpdatePostLog(
        string recordKey,
        string repository,
        string mastodonInstanceDomain = "",
        string mastodonStatusId = "",
        long telegramChatId = 0,
        int? telegramMessageCount = null,
        int telegramMessageId = 0,
        string xPostId = ""
    )
    {
        using (var db = new Database())
        {
            PostLog postLog = new PostLog();

            postLog = await db
                .PostLogs
                .Where(pl => pl.Repository == repository)
                .Where(pl => pl.RecordKey == recordKey)
                .FirstOrDefaultAsync();

            if (postLog != null)
            {
                if(!String.IsNullOrEmpty(mastodonInstanceDomain))
                    postLog.Mastodon_InstanceDomain = mastodonInstanceDomain;
                if(!String.IsNullOrEmpty(mastodonStatusId))
                    postLog.Mastodon_StatusId = mastodonStatusId;
                if(telegramChatId != 0)
                    postLog.Telegram_ChatId = telegramChatId;
                if(telegramMessageCount != null)
                    postLog.Telegram_MessageCount = telegramMessageCount;
                if(telegramMessageId != 0)
                    postLog.Telegram_MessageId = telegramMessageId;
                if(!String.IsNullOrEmpty(xPostId))
                    postLog.X_PostId = xPostId;

                postLog.UpdatedAt = DateTime.Now;
                postLog.Version = postLog.Version + 1;

                await db.SaveChangesAsync();
                return postLog;
            }

            return postLog;
        }
    }
}