namespace Booski;
using System;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Booski.Clients;
using Booski.Common;
using Booski.Data;
using Booski.Lib;
using Booski.Utilities;
using MastoLib = Mastonet;
using Telegram.Bot;
using XLib = LinqToTwitter;

public class Program
{
    public class Options
    {
        [Option('u', "username", Required = true)]
        public string BskyUsername { get; set; }
        [Option('p', "password", Required = true)]
        public string BskyPassword { get; set; }
        [Option('d', "host", Default = "public.api.bsky.app")]
        public string BskyHost { get; set; }
        [Option("mastodon-instance")]
        public string MastodonInstance { get; set; }
        [Option("mastodon-token")]
        public string MastodonToken { get; set; }
        [Option("telegram-channel")]
        public string TelegramChannel { get; set; }
        [Option("telegram-token")]
        public string TelegramToken { get; set; }
        [Option("x-api-key")]
        public string XApiKey { get; set; }
        [Option("x-api-secret")]
        public string XApiSecret { get; set; }
        [Option("x-access-token")]
        public string XAccessToken { get; set; }
        [Option("x-access-secret")]
        public string XAccessSecret { get; set; }
        [Option("horny-only-x", Hidden = true)]
        public bool HornyOnlyOnX { get; set; }
        [Option("sleep-time", Default = 10000)]
        public int SleepTime { get; set; }
        [Option("sleep-time-fetch", Default = 1000, Hidden = true)]
        public int SleepTimeFetch { get; set; }
        [Option("run-once", Hidden = true)]
        public bool RunOnceThenExit { get; set; }
    }

    public static async Task Main(string[] args)
    {
        try
        {
            await Parser.Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(Run);
        }
        catch (Exception e)
        {
            //Say.Error(e);
            throw;
            Environment.Exit(1);
        }
    }

    static async Task Run(Options o)
    {
        ServiceCollection services = new();
        services.AddBooskiLib();
        await using ServiceProvider sp = services.BuildServiceProvider();

        var atproto = sp.GetRequiredService<Booski.Lib.AtProto>();
        var atprotoRepo = sp.GetRequiredService<Booski.Lib.Lexicon.Com.Atproto.Repo>();
        var atprotoSync = sp.GetRequiredService<Booski.Lib.Lexicon.Com.Atproto.Sync>();
        var bskyActor = sp.GetRequiredService<Booski.Lib.Lexicon.App.Bsky.Actor>();

        MastoLib.MastodonClient mastodonClient = null;
        TelegramBotClient? telegramBot = null;
        XLib.TwitterContext? xClient = null;

        Booski.Lib.Internal.AppBsky.Common.Actor bskyProfile = null;
        MastoLib.Entities.InstanceV2 mastodonInstance = null;

        bool bskyConnected = false;
        bool mastodonConnected = false;
        bool telegramConnected = false;
        bool xConnected = false;

        Say.Info("Migrating database...");
        Database.Migrate();

        await atproto.CreateSession(o.BskyUsername, o.BskyPassword, o.BskyHost);

        if (atproto.GetSession() != null)
        {
            var bskyProfileResponse = await bskyActor.GetProfile(o.BskyUsername);
            bskyProfile = bskyProfileResponse.Data;

            if (!String.IsNullOrEmpty(bskyProfile.Did))
            {
                bskyConnected = true;
                Say.Success($"Connected to Bluesky ({bskyProfile.Did})");
            }
            else
                Say.Warning("Unable to connect to Bluesky");
        }

        if (
            !String.IsNullOrEmpty(o.MastodonInstance) &&
            !String.IsNullOrEmpty(o.MastodonToken)
        )
        {
            var mastodonHttpClient = new HttpClient();
            mastodonHttpClient.DefaultRequestHeaders.Add("User-Agent", "BooskiPostRelay/0");

            var mastodonAuthClient = new MastoLib.AuthenticationClient(o.MastodonInstance, mastodonHttpClient);
            mastodonClient = new MastoLib.MastodonClient(o.MastodonInstance, o.MastodonToken, mastodonHttpClient);

            mastodonInstance = await mastodonClient.GetInstanceV2();
            var mastodonUser = await mastodonClient.GetCurrentUser();

            if (!String.IsNullOrEmpty(mastodonUser.Id))
            {
                mastodonConnected = true;
                Say.Success($"Connected to Mastodon ({mastodonUser.Id})");
            }
            else
                Say.Warning("Unable to connect to Mastodon");
        }

        if (
            !String.IsNullOrEmpty(o.TelegramChannel) &&
            !String.IsNullOrEmpty(o.TelegramToken)
        )
        {
            telegramBot = new TelegramBotClient(o.TelegramToken);
            var telegramMe = await telegramBot.GetMeAsync();

            if (telegramMe.Id != 0)
            {
                telegramConnected = true;
                Say.Success($"Connected to Telegram ({telegramMe.Id})");
            }
            else
                Say.Warning("Unable to connect to Telegram");
        }

        if (
            !String.IsNullOrEmpty(o.XAccessToken) &&
            !String.IsNullOrEmpty(o.XAccessSecret) &&
            !String.IsNullOrEmpty(o.XApiKey) &&
            !String.IsNullOrEmpty(o.XApiSecret)
        )
        {
            var xClientAuth = new XLib.OAuth.SingleUserAuthorizer
            {
                CredentialStore = new XLib.OAuth.SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = o.XApiKey,
                    ConsumerSecret = o.XApiSecret,
                    AccessToken = o.XAccessToken,
                    AccessTokenSecret = o.XAccessSecret
                }
            };

            try
            {
                xClient = new XLib.TwitterContext(xClientAuth);

                var xAccount = xClient
                    .Account
                    .Where(a => a.Type == XLib.AccountType.VerifyCredentials)
                    .FirstOrDefault();
                var xUsername = xAccount.User.ScreenNameResponse;

                xConnected = true;
                Say.Success($"Connected to X (@{xUsername})");
            }
            catch (Exception e)
            {
                Say.Warning("Unable to connect to X", e.Message);
            }
        }

        while (true)
        {
            List<Post> posts = new List<Post>();
            List<PostLog> postLogs = await PostLogs.GetPostLogs(bskyProfile.Did);
            int postLogsCount = await PostLogs.CountPostLogs(bskyProfile.Did);
            bool firstRun = postLogsCount == 0;

            string recordCursor = "";

            Say.Info($"Fetching posts...");

            while (true)
            {
                var recordsRequest = await atprotoRepo.ListRecords(
                    "app.bsky.feed.post", bskyProfile.Did,
                    cursor: recordCursor,
                    limit: 100
                );

                if (recordsRequest.Data.Records.Count() != 0)
                {
                    foreach (var record in recordsRequest.Data.Records)
                    {
                        if (record.Value.GetType() == typeof(Booski.Lib.Polymorphs.AppBsky.FeedPost))
                        {
                            var recordValue = record.Value as Booski.Lib.Polymorphs.AppBsky.FeedPost;
                            var newPost = new Post
                            {
                                Profile = bskyProfile,
                                Record = recordValue,
                                RecordKey = record.Uri.Split('/').Last(),
                                Uri = record.Uri
                            };
                            posts.Add(newPost);
                        }
                    }

                    recordCursor = recordsRequest.Data.Cursor;
                }
                else
                {
                    break;
                }

                Thread.Sleep(o.SleepTimeFetch);
            }

            Say.Success($"Fetched {posts.Count()} posts");

            if (!firstRun)
            {
                foreach (var postLog in postLogs)
                {
                    var foundPost = posts
                        .Where(p => p.RecordKey == postLog.RecordKey)
                        .FirstOrDefault();

                    if (foundPost == null)
                    {
                        bool deleteSuccess = true;

                        if (
                            mastodonConnected &&
                            postLog.Mastodon_InstanceDomain != null &&
                            postLog.Mastodon_StatusId != null
                        )
                        {
                            string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.Mastodon_InstanceDomain}/{postLog.Mastodon_StatusId})";

                            if (postLog.Mastodon_InstanceDomain != mastodonInstance.Domain)
                            {
                                deleteSuccess = false;
                                Say.Warning($"Not deleting from Mastodon: {consoleMessageSuffix}", $"Current instance domain ({mastodonInstance.Domain} does not match logged domain ({postLog.Mastodon_InstanceDomain}))");
                            }
                            else
                            {
                                try
                                {
                                    await MastodonClient.DeleteFromMastodon(mastodonClient, postLog.Mastodon_StatusId);
                                    Say.Success($"Deleted from Mastodon: {consoleMessageSuffix}");
                                }
                                catch (Exception e)
                                {
                                    deleteSuccess = false;
                                    Say.Warning($"Unable to delete from Mastodon: {consoleMessageSuffix}", e.Message);
                                }
                            }
                        }

                        if (
                            telegramConnected &&
                            postLog.Telegram_ChatId != null &&
                            postLog.Telegram_MessageCount != null &&
                            postLog.Telegram_MessageId != null
                        )
                        {
                            string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.Telegram_ChatId}/{postLog.Telegram_MessageId})";
                            int currentMessageId = (int)postLog.Telegram_MessageId;
                            int lastMessageId = (int)postLog.Telegram_MessageId + (int)postLog.Telegram_MessageCount;

                            while (currentMessageId != lastMessageId)
                            {
                                try
                                {
                                    await TelegramClient.DeleteFromTelegram(
                                        client: telegramBot,
                                        telegramChatId: (long)postLog.Telegram_ChatId,
                                        telegramMessageId: currentMessageId
                                    );
                                    Say.Success($"Deleted from Telegram: {consoleMessageSuffix}");
                                }
                                catch (Exception e)
                                {
                                    deleteSuccess = false;
                                    Say.Warning($"Unable to delete from Telegram: {consoleMessageSuffix}", e.Message);
                                }

                                currentMessageId++;
                            }
                        }

                        if (
                            xConnected &&
                            postLog.X_PostId != null
                        )
                        {
                            string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.X_PostId})";

                            try
                            {
                                await XClient.DeleteFromX(
                                    client: xClient,
                                    postLog.X_PostId
                                );

                                Say.Success($"Deleted from X: {consoleMessageSuffix}");
                            }
                            catch (Exception e)
                            {
                                deleteSuccess = false;
                                Say.Warning($"Unable to delete from X: {consoleMessageSuffix}", e.Message);
                            }
                        }

                        if (deleteSuccess)
                        {
                            Say.Success($"Deleted: {postLog.RecordKey}");
                            await PostLogs.DeletePostLog(postLog.RecordKey, bskyProfile.Did);
                        }
                    }
                }
            }

            if (firstRun)
                Say.Warning($"First run. Caching and ignoring all previous posts");

            foreach (var post in posts)
            {
                var postLog = await PostLogs.GetPostLog(post.RecordKey, bskyProfile.Did);
                if (postLog == null)
                {
                    postLog = await PostLogs.AddPostLog(
                        ignored: firstRun,
                        recordKey: post.RecordKey,
                        repository: bskyProfile.Did
                    );

                    if (!firstRun)
                        Say.Success($"Added: {postLog.RecordKey}");
                }

                Embed embed = new Embed();
                PostLog? replyParentPostLog = null;

                if (postLog.Ignored)
                    continue;

                if (post.Record.Embed != null)
                {
                    Type embedType = post.Record.Embed.GetType();

                    if (
                        embedType == typeof(Booski.Lib.Polymorphs.AppBsky.EmbedRecord) ||
                        post.Record.Embed.GetType() == typeof(Booski.Lib.Polymorphs.AppBsky.EmbedRecordWithMedia)
                    )
                    {
                        string recordEmbedCid = "";
                        string recordEmbedUri = "";

                        switch (embedType)
                        {
                            case Type when embedType == typeof(Booski.Lib.Polymorphs.AppBsky.EmbedRecord):
                                var recordEmbed = post.Record.Embed as Booski.Lib.Polymorphs.AppBsky.EmbedRecord;
                                recordEmbedCid = recordEmbed.Record.Cid;
                                recordEmbedUri = recordEmbed.Record.Uri;
                                break;
                            case Type when embedType == typeof(Booski.Lib.Polymorphs.AppBsky.EmbedRecordWithMedia):
                                var recordEmbedWithMedia = post.Record.Embed as Booski.Lib.Polymorphs.AppBsky.EmbedRecordWithMedia;
                                var recordEmbedWithMediaRecord = recordEmbedWithMedia.Record as Booski.Lib.Polymorphs.AppBsky.EmbedRecord;
                                recordEmbedCid = recordEmbedWithMediaRecord.Record.Cid;
                                recordEmbedUri = recordEmbedWithMediaRecord.Record.Uri;
                                embed = BskyUtilities.ParseEmbeds(recordEmbedWithMedia.Media, bskyProfile.Did);
                                break;
                        }

                        post.Record.Reply = new Booski.Lib.Internal.AppBsky.Common.Reply
                        {
                            Parent = new Booski.Lib.Internal.AppBsky.Common.ReplyMeta
                            {
                                Cid = recordEmbedCid,
                                Uri = recordEmbedUri
                            }
                        };
                    }
                    else
                    {
                        embed = BskyUtilities.ParseEmbeds(post.Record.Embed, bskyProfile.Did);

                        if (embed.Items.Count() == 0)
                        {
                            Say.Warning($"Ignoring: {postLog.RecordKey}", "Post has embeds but none are supported");
                            postLog = await PostLogs.IgnorePostLog(postLog.RecordKey, bskyProfile.Did);
                        }
                    }
                }

                if (post.Record.Reply != null)
                {
                    var parentRecordKey = post.Record.Reply.Parent.Uri.Split('/').Last();
                    var parentPostLog = await PostLogs.GetPostLog(parentRecordKey, bskyProfile.Did);

                    if (parentPostLog == null)
                    {
                        Say.Warning($"Ignoring: {postLog.RecordKey}", "Post is a reply, but parent doesn't exist (either deleted or not ours)");
                        postLog = await PostLogs.IgnorePostLog(postLog.RecordKey, bskyProfile.Did);
                    }
                    else
                    {
                        replyParentPostLog = parentPostLog;
                    }
                }

                if (post.Record.Text.StartsWith("@")) // TODO: Check if this is a real mention?
                {
                    Say.Warning($"Ignoring: {postLog.RecordKey}", "Post starts with \"@\"");
                    postLog = await PostLogs.IgnorePostLog(postLog.RecordKey, bskyProfile.Did);
                }

                if (postLog.Ignored)
                    continue;

                if (post.Record.Labels != null)
                    post.Sensitive = !String.IsNullOrEmpty(BskyUtilities.GetContentWarning(post.Record.Labels));

                if (
                    mastodonConnected &&
                    postLog.Mastodon_InstanceDomain == null &&
                    postLog.Mastodon_StatusId == null
                )
                {
                    string? replyToStatusId = null;
                    MastoLib.Entities.Status mastodonMessage = null;

                    if (replyParentPostLog != null)
                        if (replyParentPostLog.Deleted != true)
                            replyToStatusId = replyParentPostLog.Mastodon_StatusId;

                    try
                    {
                        mastodonMessage = await MastodonClient.PostToMastodon(
                            client: mastodonClient,
                            embed: embed,
                            reply: replyToStatusId,
                            sensitive: post.Sensitive,
                            text: await MastodonUtilities.GenerateStatusText(post)
                        );
                    }
                    catch (Exception e)
                    {
                        Say.Warning($"Unable to post to Mastodon: {post.RecordKey}", e.Message);
                    }

                    if (mastodonMessage != null)
                    {
                        postLog = await PostLogs.UpdatePostLog(
                            recordKey: postLog.RecordKey,
                            repository: bskyProfile.Did,
                            mastodonInstanceDomain: mastodonInstance.Domain,
                            mastodonStatusId: mastodonMessage.Id
                        );

                        Say.Success($"Posted to Mastodon: {postLog.RecordKey} ({postLog.Mastodon_InstanceDomain}/{postLog.Mastodon_StatusId})");
                    }
                }

                if (
                    telegramConnected &&
                    postLog.Telegram_ChatId == null &&
                    postLog.Telegram_MessageCount == null &&
                    postLog.Telegram_MessageId == null
                )
                {
                    int? replyToMessageId = null;
                    List<Telegram.Bot.Types.Message>? telegramMessage = null;

                    if (replyParentPostLog != null)
                        if (replyParentPostLog.Deleted != true)
                            replyToMessageId = replyParentPostLog.Telegram_MessageId;

                    try
                    {
                        telegramMessage = await TelegramClient.PostToTelegram(
                            client: telegramBot,
                            chatId: o.TelegramChannel,
                            embed: embed,
                            reply: replyToMessageId,
                            text: await TelegramUtilities.GenerateCaption(post)
                        );
                    }
                    catch (Exception e)
                    {
                        throw;
                        Say.Warning($"Unable to post to Telegram: {post.RecordKey}", e.Message);
                    }

                    if (telegramMessage != null)
                    {
                        var firstTelegramMessage = telegramMessage.FirstOrDefault();

                        postLog = await PostLogs.UpdatePostLog(
                            recordKey: postLog.RecordKey,
                            repository: bskyProfile.Did,
                            telegramChatId: firstTelegramMessage.Chat.Id,
                            telegramMessageCount: telegramMessage.Count(),
                            telegramMessageId: firstTelegramMessage.MessageId
                        );

                        Say.Success($"Posted to Telegram: {postLog.RecordKey} ({postLog.Telegram_ChatId}/{postLog.Telegram_MessageId})");
                    }
                }

                if (
                    xConnected &&
                    postLog.X_PostId == null
                )
                {
                    string? replyToPostId = null;
                    XLib.Tweet xMessage = null;

                    if(o.HornyOnlyOnX && !post.Sensitive)
                        continue;
                    
                    if (replyParentPostLog != null)
                        if (replyParentPostLog.Deleted != true)
                            replyToPostId = replyParentPostLog.X_PostId;

                    try
                    {
                        xMessage = await XClient.PostToX(
                            client: xClient,
                            embed: embed,
                            reply: replyToPostId,
                            text: await XUtilities.GeneratePostText(post)
                        );
                    }
                    catch (Exception e)
                    {
                        throw;
                        Say.Warning($"Unable to post to X: {post.RecordKey}", e.Message);
                    }

                    if (xMessage != null)
                    {
                        postLog = await PostLogs.UpdatePostLog(
                            recordKey: postLog.RecordKey,
                            repository: bskyProfile.Did,
                            xPostId: xMessage.ID
                        );

                        Say.Success($"Posted to X: {postLog.RecordKey} ({postLog.X_PostId})");
                    }
                }
            }

            if (o.RunOnceThenExit)
                Environment.Exit(0);
            else
                Thread.Sleep(o.SleepTime);
        }
    }
}