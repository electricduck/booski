using Booski.Common.Options;
using Booski.Contexts;
using Booski.Data;
using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;
using BskyApi = Booski.Lib.Xrpc;

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

    private readonly int NostrNpubLength = 63;

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
#pragma warning disable CS8604 // Possible null reference argument.
                    await AddOrUpdateMapEntry(o, bskyProfile);
#pragma warning restore CS8604 // Possible null reference argument.
                    break;
                case Enums.UsernameMapCommandAction.Get:
#pragma warning disable CS8604 // Possible null reference argument.
                    await GetMapEntry(bskyProfile);
#pragma warning restore CS8604 // Possible null reference argument.
                    break;
                case Enums.UsernameMapCommandAction.Remove:
#pragma warning disable CS8604 // Possible null reference argument.
                    await RemoveMapEntry(bskyProfile);
#pragma warning restore CS8604 // Possible null reference argument.
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

    async Task AddOrUpdateMapEntry(UsernameMapOptions o, AppBskyLexicons.Actor.Defs_ProfileViewDetailed bskyProfile)
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

        // > "Nostr is easy"
        // > also Nostr:
        if(o.NostrHandle != null)
        {
            if(o.NostrHandle.StartsWith("npub"))
            {
                if(o.NostrHandle.Length != NostrNpubLength)
                    Say.Warning("Nostr Public Key (--nostr) is invalid", $"Expected {NostrNpubLength} characters, got {o.NostrHandle.Length}", separate: true);
            }
            else
            {
                if(!o.NostrHandle.Contains("@"))
                    Say.Warning("Nostr NIP-05 (--nostr) is invalid", $"Expected either '@user.com' or 'user@provider.com'", separate: true);
                else
                    if(o.NostrHandle.StartsWith("@"))
                        o.NostrHandle.TrimStart('@');
            }
        }

        if(
            o.MastodonHandle == null &&
            o.NostrHandle == null &&
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
            nostrHandle: o.NostrHandle,
            telegramHandle: o.TelegramHandle,
            threadsHandle: o.ThreadsHandle,
            xHandle: o.XHandle
        );

        Say.Success($"Updated maps for @{bskyProfile.Handle} ({bskyDid})");
        await GetMapEntry(bskyProfile);
        
    }

    async Task GetMapEntry(AppBskyLexicons.Actor.Defs_ProfileViewDetailed bskyProfile)
    {
        var bskyDid = bskyProfile.Did;

        string? mastodonHandle = await UsernameMaps.GetMastodonHandleForDid(bskyDid);
        string? nostrHandle = await UsernameMaps.GetNostrHandleForDid(bskyDid);
        string? telegramHandle = await UsernameMaps.GetTelegramHandleForDid(bskyDid);
        string? threadsHandle = await UsernameMaps.GetThreadsHandleForDid(bskyDid);
        string? xHandle = await UsernameMaps.GetXHandleForDid(bskyDid);

        mastodonHandle = String.IsNullOrEmpty(mastodonHandle) ? "(None)" : $"@{mastodonHandle}";
        nostrHandle = String.IsNullOrEmpty(nostrHandle) ? "(None)" : $"@{nostrHandle}";
        telegramHandle = String.IsNullOrEmpty(telegramHandle) ? "(None)" : $"@{telegramHandle}";
        threadsHandle = String.IsNullOrEmpty(threadsHandle) ? "(None)" : $"@{threadsHandle}";
        xHandle = String.IsNullOrEmpty(xHandle) ? "(None)" : $"@{xHandle}";

        string outputHeader = $"@{bskyProfile.Handle} ({bskyDid})";

string outputBody = $@"↳ Mastodon: {mastodonHandle}
↳ Nostr:    {nostrHandle}
↳ Telegram: {telegramHandle}
↳ Threads:  {threadsHandle}
↳ X:        {xHandle}";

        Say.Custom(outputHeader, outputBody, "🔀");
    }

    async Task RemoveMapEntry(AppBskyLexicons.Actor.Defs_ProfileViewDetailed bskyProfile)
    {
        var bskyDid = bskyProfile.Did;

        await UsernameMaps.AddOrUpdateUsernameMap(
            did: bskyDid,
            mastodonHandle: null,
            nostrHandle: null,
            telegramHandle: null,
            threadsHandle: null,
            xHandle: null
        );

        Say.Success($"Removed maps for @{bskyProfile.Handle} ({bskyDid})");
    }

    async Task<AppBskyLexicons.Actor.Defs_ProfileViewDetailed?> ResolveBskyProfileFromHandle(string handle)
    {
        AppBskyLexicons.Actor.Defs_ProfileViewDetailed? resolvedProfile = null;

        if (
            !_bskyContext.IsConnected &&
            Program.Config != null &&
            Program.Config.Clients != null
        )
            await _bskyContext.CreateSession(Program.Config.Clients);

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
        catch
        {
            throw new Exception($"Unable to find profile for '{handle}'");
        }

        return resolvedProfile;
    }
}