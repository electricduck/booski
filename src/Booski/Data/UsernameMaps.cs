using Booski.Common;
using Microsoft.EntityFrameworkCore;

namespace Booski.Data;

public class UsernameMaps
{
    public async static Task<UsernameMap> AddOrUpdateUsernameMap(
        string did,
        string mastodonHandle,
        string telegramHandle,
        string threadsHandle,
        string xHandle
    )
    {
        using (var db = new Database())
        {
            UsernameMap usernameMap = new UsernameMap();

            var existingUsernameMap = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if (existingUsernameMap == null)
            {
                usernameMap = new UsernameMap
                {
                    Bluesky_Did = did,
                    Mastodon_Handle = mastodonHandle,
                    Telegram_Handle = telegramHandle,
                    Threads_Handle = threadsHandle,
                    X_Handle = xHandle
                };

                await db.UsernameMaps.AddAsync(usernameMap);
                await db.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("updating!");

                existingUsernameMap.Mastodon_Handle = String.IsNullOrEmpty(mastodonHandle) ? "" : mastodonHandle;
                existingUsernameMap.Telegram_Handle = String.IsNullOrEmpty(telegramHandle) ? "" : telegramHandle;
                existingUsernameMap.Threads_Handle = String.IsNullOrEmpty(threadsHandle) ? "" : threadsHandle;
                existingUsernameMap.X_Handle = String.IsNullOrEmpty(xHandle) ? "" : xHandle;
                
                await db.SaveChangesAsync();
            }

            return usernameMap;
        }
    }

    public async static Task<bool> DoesUsernameMapExist(
        string did
    )
    {
        using (var db = new Database())
        {
            var existingUsernameMap = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if(existingUsernameMap != null)
                return true;
            else
                return false;
        }
    }

    public async static Task<string> GetMastodonHandleForDid(
        string did
    )
    {
        using (var db = new Database())
        {
            string mastodonHandle = "";

            var foundDid = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if(foundDid != null)
                mastodonHandle = foundDid.Mastodon_Handle;

            return mastodonHandle;
        }
    }

    public async static Task<string> GetTelegramHandleForDid(
        string did
    )
    {
        using (var db = new Database())
        {
            string telegramHandle = "";

            var foundDid = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if(foundDid != null)
                telegramHandle = foundDid.Telegram_Handle;

            return telegramHandle;
        }
    }

    public async static Task<string> GetThreadsHandleForDid(
        string did
    )
    {
        using (var db = new Database())
        {
            string threadsHandle = "";

            var foundDid = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if(foundDid != null)
                threadsHandle = foundDid.Threads_Handle;

            return threadsHandle;
        }
    }

    public async static Task<string> GetXHandleForDid(
        string did
    )
    {
        using (var db = new Database())
        {
            string xHandle = "";

            var foundDid = await db
                .UsernameMaps
                .Where(um => um.Bluesky_Did == did)
                .FirstOrDefaultAsync();

            if(foundDid != null)
                xHandle = foundDid.X_Handle;

            return xHandle;
        }
    }
}
