using CommandLine;

namespace Booski.Common;

public class Options
    {
        // General
        [Option('c', "config-path", HelpText = "Path to directory containing configuration and database.\nIf not supplied, the OS's default locations will be used.")]
        public string ConfigPath { get; set; }
        [Option('s', "sleep-time", Default = 1, HelpText = "Interval (in seconds) between checking for new posts on Bluesky.")]
        public int SleepTime { get; set; }

        // Bluesky
        [Option("bsky-username", Required = true, HelpText = "Handle/email of your Bluesky account.")]
        public string BskyUsername { get; set; }
        [Option("bsky-password", Required = true, HelpText = "Password of your Bluesky account.\nFor safety, create a new app password at https://bsky.app/settings/app-passwords.")]
        public string BskyPassword { get; set; }
        [Option("bsky-host", Default = "public.api.bsky.app", HelpText = "Host of your Bluesky account.\n'public.api.bsky.app' may not always work; find your host from https://plc.directory.")]
        public string BskyHost { get; set; }

        // Mastodon
        [Option("mastodon-instance", HelpText = "Host of your Mastodon account.")]
        public string MastodonInstance { get; set; }
        [Option("mastodon-token", HelpText = "Token that can consume your Mastodon account.\nGenerate a token with https://takahashim.github.io/mastodon-access-token.")]
        public string MastodonToken { get; set; }

        // Telegram
        [Option("telegram-channel", HelpText = "Handle/ID for your Telegram channel.")]
        public string TelegramChannel { get; set; }
        [Option("telegram-token", HelpText = "Token for your Telegram bot.")]
        public string TelegramToken { get; set; }

        // X
        [Option("x-api-key", HelpText = "API Key for your X project.")]
        public string XApiKey { get; set; }
        [Option("x-api-secret", HelpText = "API Secret for your X project.")]
        public string XApiSecret { get; set; }
        [Option("x-access-token", HelpText = "Access Token for your X project.")]
        public string XAccessToken { get; set; }
        [Option("x-access-secret", HelpText = "Access Secret for your X project.")]
        public string XAccessSecret { get; set; }

        // Extras/Experimental
        [Option("horny-only-x", Hidden = false, HelpText = "Only post sensitive content to X.\nWhat else is this garbage platform even for these days?")]
        public bool HornyOnlyOnX { get; set; }

        // Dev
        [Option("no-check-updates", Hidden = true, HelpText = "Don't check for updates on startup.")]
        public bool DoNotCheckForUpdates { get; set; }
        [Option("sleep-time-fetch", Default = 1, Hidden = true, HelpText = "Interval (in seconds) between fetching batches of your feed (limited to 100) from Bluesky.")]
        public int SleepTimeFetch { get; set; }
        [Option("run-once", Hidden = true, HelpText = "Exit after first loop.")]
        public bool RunOnceThenExit { get; set; }
    }