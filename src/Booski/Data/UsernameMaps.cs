namespace Booski.Data;
using Microsoft.EntityFrameworkCore;

public class UsernameMaps
{
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