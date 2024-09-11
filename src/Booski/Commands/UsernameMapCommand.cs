using Booski.Common.Options;
using Booski.Contexts;
using Booski.Data;
using AppBskyResponses = Booski.Lib.Internal.AppBsky.Responses;
using BskyApi = Booski.Lib.Lexicon;

namespace Booski.Commands;

public interface IUsernameMapCommand
{
    UsernameMapOptions Options { get; set; }

    Task Invoke(UsernameMapOptions o);
}

internal sealed class UsernameMapCommand : IUsernameMapCommand
{
    public UsernameMapOptions Options { get; set; }

    private BskyApi.App.Bsky.Actor _bskyActorApi;
    private IBskyContext _bskyContext;

    public UsernameMapCommand(
        BskyApi.App.Bsky.Actor bskyActorApi,
        IBskyContext bskyContext
    )
    {
        _bskyActorApi = bskyActorApi;
        _bskyContext = bskyContext;
    }

    public async Task Invoke(UsernameMapOptions o)
    {
        Enums.UsernameMapCommandAction action = Enums.UsernameMapCommandAction.None;
        int actionsCount = 0;

        if(o.BskyUsername.StartsWith("@"))
            o.BskyUsername = o.BskyUsername.TrimStart('@');

        var bskyProfile = await ResolveBskyProfileFromHandle(o.BskyUsername);

        if(o.AddMap)
        {
            action = Enums.UsernameMapCommandAction.Add;
            actionsCount++;
        }

        if(o.GetMap)
        {
            action = Enums.UsernameMapCommandAction.Get;
            actionsCount++;
        }

        if(o.RemoveMap)
        {
            action = Enums.UsernameMapCommandAction.Remove;
            actionsCount++;
        }

        if(actionsCount == 1)
        {
            switch(action)
            {
                case Enums.UsernameMapCommandAction.Add:
                    await AddOrUpdateMapEntry(o, bskyProfile);
                    break;
                case Enums.UsernameMapCommandAction.Get:
                    await GetMapEntry(bskyProfile);
                    break;
                case Enums.UsernameMapCommandAction.Remove:
                    await RemoveMapEntry(bskyProfile);
                    break;
            }
        }
        else if (actionsCount == 0)
        {
            Say.Error("No action option specified", "Use --add/-a, --get/-g or --remove/-r. See --help/-h for more");
        }
        else
        {
            Say.Error("Multiple action options specified", "Use --add/-a, --get/-g or --remove/-r. See --help/-h for more");
        }
    }

    async Task AddOrUpdateMapEntry(UsernameMapOptions o, AppBskyResponses.GetProfileResponse bskyProfile)
    {
        var bskyDid = bskyProfile.Did;

        // TODO: Validate
        if(o.MastodonHandle != null && o.MastodonHandle.StartsWith("@"))
            o.MastodonHandle = o.MastodonHandle.TrimStart('@');
        if(o.TelegramHandle != null && o.TelegramHandle.StartsWith("@"))
            o.TelegramHandle = o.TelegramHandle.TrimStart('@');
        if(o.ThreadsHandle != null && o.ThreadsHandle.StartsWith("@"))
            o.ThreadsHandle = o.ThreadsHandle.TrimStart('@');
        if(o.XHandle != null && o.XHandle.StartsWith("@"))
            o.XHandle = o.XHandle.TrimStart('@');

        if(
            o.MastodonHandle == null &&
            o.TelegramHandle == null &&
            o.ThreadsHandle == null &&
            o.XHandle == null
        )
        {
            await RemoveMapEntry(bskyProfile);
            return;
        }

        var usernameMap = await UsernameMaps.AddOrUpdateUsernameMap(
            did: bskyDid,
            mastodonHandle: o.MastodonHandle,
            telegramHandle: o.TelegramHandle,
            threadsHandle: o.ThreadsHandle,
            xHandle: o.XHandle
        );

        Say.Success($"Updated maps for @{bskyProfile.Handle} ({bskyDid})");
        await GetMapEntry(bskyProfile);
        
    }

    async Task GetMapEntry(AppBskyResponses.GetProfileResponse bskyProfile)
    {
        var bskyDid = bskyProfile.Did;

        string mastodonHandle = await UsernameMaps.GetMastodonHandleForDid(bskyDid);
        string telegramHandle = await UsernameMaps.GetTelegramHandleForDid(bskyDid);
        string threadsHandle = await UsernameMaps.GetThreadsHandleForDid(bskyDid);
        string xHandle = await UsernameMaps.GetXHandleForDid(bskyDid);

        mastodonHandle = String.IsNullOrEmpty(mastodonHandle) ? "(None)" : $"@{mastodonHandle}";
        telegramHandle = String.IsNullOrEmpty(telegramHandle) ? "(None)" : $"@{telegramHandle}";
        threadsHandle = String.IsNullOrEmpty(threadsHandle) ? "(None)" : $"@{threadsHandle}";
        xHandle = String.IsNullOrEmpty(xHandle) ? "(None)" : $"@{xHandle}";

        string outputHeader = $"@{bskyProfile.Handle} ({bskyDid})";

string outputBody = $@"↳ Mastodon: {mastodonHandle}
↳ Telegram: {telegramHandle}
↳ Threads:  {threadsHandle}
↳ X:        {xHandle}";

        Say.Custom(outputHeader, outputBody, "🔀");
    }

    async Task RemoveMapEntry(AppBskyResponses.GetProfileResponse bskyProfile)
    {
        var bskyDid = bskyProfile.Did;

        await UsernameMaps.AddOrUpdateUsernameMap(
            did: bskyDid,
            mastodonHandle: null,
            telegramHandle: null,
            threadsHandle: null,
            xHandle: null
        );

        Say.Success($"Removed maps for @{bskyProfile.Handle} ({bskyDid})");
    }

    async Task<AppBskyResponses.GetProfileResponse> ResolveBskyProfileFromHandle(string handle)
    {
        AppBskyResponses.GetProfileResponse resolvedProfile = null;

        if (!_bskyContext.IsConnected)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await _bskyContext.CreateSession(Program.Config.Clients);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (!_bskyContext.IsConnected)
        {
            // TODO: Throw exception
            Say.Error("Unable to connect to Bluesky", "To resolve DIDs, a valid login to Bluesky is required");
            Program.Exit(true);
        }

        if (handle.StartsWith("@"))
            handle = handle.Replace("@", "");

        try
        {
            var resolvedProfileResponse = await _bskyActorApi.GetProfile(handle);
            resolvedProfile = resolvedProfileResponse.Data;
        }
        catch (Exception e)
        {
            // TODO: Throw exception
            Say.Error($"Unable to find profile for '{handle}'");
            Program.Exit(true);
        }

        return resolvedProfile;
    }
}