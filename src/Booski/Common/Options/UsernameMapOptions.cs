using CommandLine;
using Booski.Utilities;

namespace Booski.Common.Options;

[Verb("usernames", HelpText = "Map Bluesky usernames to other services.")]
public class UsernameMapOptions : GlobalOptions
{
    [Option('u', "username", Required = true, HelpText = "Handle or DID of Bluesky user to operate on.\nExamples: @jay.bsky.team, did:plc:oky5czdrnfjpqslsw2a5iclo.")]
    public string BskyUsername { get; set; }

    [Option('a', "add", HelpText = "Add or update username mapping.\nAll usernames to map to must be supplied; empty arguments are treated as removals.")]
    public bool AddMap { get; set; }
    [Option('r', "remove", HelpText = "Remove username mapping.\nOnly --username needs to be supplied; all other arguments are ignored.")]
    public bool RemoveMap { get; set; }
    [Option('g', "get", HelpText = "Print username mapping.\nOnly --username needs to be supplied; all other arguments are ignored.")]
    public bool GetMap { get; set; }

    [Option("mastodon", HelpText = "Handle of Mastodon/ActivityPub user to map Bluesky user to.\nExample: @Gargron@mastodon.social.")]
    public string? MastodonHandle { get; set; }
    [Option("nostr", HelpText = "Handle (Public Key or NIP-05) of Nostr user to map Bluesky user to.\nExample: npub1sn0wdenkukak0d9dfczzeacvhkrgz92ak56egt7vdgzn8pv2wfqqhrjdv9, @fiatjaf.com, mina@zaps.lol.\nNote: Nostr is currently not supported; this option is intended for futureproofing.")]
    public string? NostrHandle { get; set; }
    [Option("telegram", HelpText = "Handle of Telegram user to map Bluesky user to.\nExample: @durov.\nNote: Instead of mentioning the user themselves it is more appropriate — and potentially less spammy — to use their Personal Channel instead.")]
    public string? TelegramHandle { get; set; }
    [Option("threads", HelpText = "Handle of Threads user to map Bluesky user to.\nExample: @mosseri.\nNote: Threads is currently not supported; this option is intended for futureproofing.")]
    public string? ThreadsHandle { get; set; }
    [Option("xapp", HelpText = "Handle of X user to map Bluesky user to.\nExample: @elonmusk.")]
    public string? XHandle { get; set; }
}