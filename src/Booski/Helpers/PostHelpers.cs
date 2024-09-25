using Booski.Common;
using Booski.Contexts;
using Booski.Data;
using Booski.Enums;

namespace Booski.Helpers;

public interface IPostHelpers
{
    bool HornyOnlyOnX { get; set; }

    Task BuildPostCache(int fetchSleep);
    Task SyncAddedPosts(int syncSleep, bool retryIgnored);
    Task SyncDeletedPosts(int syncSleep);
}

internal sealed class PostHelpers : IPostHelpers
{
    public bool HornyOnlyOnX { get; set; } // HACK: Until we have filters

    private int OldPostHours { get; set; } = 24;
    private List<Post>? PostCache { get; set; }

    private IBskyContext _bskyContext;
    private IBskyHelpers _bskyHelpers;
    private II18nHelpers _i18nHelpers;
    private IMastodonContext _mastodonContext;
    private IMastodonHelpers _mastodonHelpers;
    private INostrContext _nostrContext;
    private ITelegramContext _telegramContext;
    private ITelegramHelpers _telegramHelpers;
    private IXContext _xContext;
    private IXHelpers _xHelpers;

    public PostHelpers(
        IBskyContext bskyContext,
        IBskyHelpers bskyHelpers,
        II18nHelpers i18nHelpers,
        IMastodonContext mastodonContext,
        IMastodonHelpers mastodonHelpers,
        INostrContext nostrContext,
        ITelegramContext telegramContext,
        ITelegramHelpers telegramHelpers,
        IXContext xContext,
        IXHelpers xHelpers
    )
    {
        _bskyContext = bskyContext;
        _bskyHelpers = bskyHelpers;
        _i18nHelpers = i18nHelpers;
        _mastodonContext = mastodonContext;
        _mastodonHelpers = mastodonHelpers;
        _nostrContext = nostrContext;
        _telegramContext = telegramContext;
        _telegramHelpers = telegramHelpers;
        _xContext = xContext;
        _xHelpers = xHelpers;
    }

    public async Task BuildPostCache(int fetchSleep)
    {
        Say.Debug("[PostHelpers.BuildPostCache] Fetching posts...");

        string cursor = "";
        List<Post> posts = new List<Post>();

        while (true)
        {
            var feed = await _bskyHelpers.GetProfileFeed(cursor);

            if (feed.Records.Count() != 0)
            {
                foreach (var record in feed.Records)
                {
                    if (record.Value.GetType() == typeof(Lib.Polymorphs.AppBsky.FeedPost))
                    {
                        var recordValue = record.Value as Lib.Polymorphs.AppBsky.FeedPost;

                        if (
                            _bskyContext.State != null &&
                            recordValue != null
                        )
                        {
                            var post = new Post
                            {
                                Profile = _bskyContext.State.Profile,
                                Record = recordValue,
                                RecordKey = record.Uri.Split('/').Last(),
                                Uri = record.Uri
                            };
                            posts.Add(post);
                        }
                    }
                }

                cursor = feed.Cursor;
            }
            else
            {
                break;
            }

            Thread.Sleep(fetchSleep);
        }

        posts = posts.OrderByDescending(p => p.RecordKey).ToList();
        PostCache = posts;
    }

    public async Task SyncAddedPosts(int syncSleep, bool retryIgnored)
    {
        bool firstRun = await IsFirstRun();

        if (firstRun)
            Say.Warning($"First run. Caching and ignoring all previous posts");

        if (PostCache == null)
            return;

        foreach (var post in PostCache)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var postLog = await PostLogs.GetPostLog(post.RecordKey, _bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (postLog == null)
            {
                postLog = await PostLogs.AddOrUpdatePostLog(
                    ignored: firstRun ? IgnoredReason.FirstRun : IgnoredReason.None,
                    recordKey: post.RecordKey,
                    repository: _bskyContext.State.Did
                );

                if (!firstRun)
                    Say.Success($"Added: {postLog.RecordKey}");
            }

            if (
                retryIgnored &&
                postLog.Ignored != IgnoredReason.None &&
                postLog.Ignored != IgnoredReason.FirstRun
            )
            {
                postLog = await PostLogs.IgnorePostLog(
                    reason: IgnoredReason.None,
                    recordKey: post.RecordKey,
                    repository: _bskyContext.State.Did
                );
            }

            if (postLog != null)
            {
                postLog = await UpdatePostLogIgnored(post, postLog);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (postLog.Ignored != IgnoredReason.None)
                    continue;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            Embed? embed = await GetEmbedForPost(post);
            PostLog? replyParentPostLog = await GetReplyParentForPost(post);

            if (post.Record.Langs != null && post.Record.Langs.Length > 0)
                post.Language = _i18nHelpers.GetLanguageForLang(post.Record.Langs[0]);
            else
                post.Language = Language.En;

            if (post.Record.Labels != null)
                post.Sensitivity = _bskyHelpers.ParseLabels(post.Record.Labels);

            if (
                _mastodonContext.IsConnected &&
                postLog != null &&
                postLog.Mastodon_InstanceDomain == null &&
                postLog.Mastodon_StatusId == null
            )
                await SyncAddedPostWithMastodon(postLog, post, embed, replyParentPostLog);

            if (
                _telegramContext.IsConnected &&
                postLog != null &&
                postLog.Telegram_ChatId == null &&
                postLog.Telegram_MessageCount == null &&
                postLog.Telegram_MessageId == null
            )
                await SyncAddedPostWithTelegram(postLog, post, embed, replyParentPostLog);

            if (
                _xContext.IsConnected &&
                postLog != null &&
                postLog.X_PostId == null
            )
            {
                // BUG: If the user forgets to pass --horny-only-x, their X will be irreversibly flooded
                if (HornyOnlyOnX && post.Sensitivity == Enums.Sensitivity.None)
                    continue;

                await SyncAddedPostWithX(postLog, post, embed, replyParentPostLog);
            }

            Thread.Sleep(syncSleep);
        }
    }

    public async Task SyncDeletedPosts(int syncSleep)
    {
        if (!await IsFirstRun())
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            List<PostLog> postLogs = await PostLogs.GetPostLogs(_bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            foreach (var postLog in postLogs)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                var foundPost = PostCache
                    .Where(p => p.RecordKey == postLog.RecordKey)
                    .FirstOrDefault();
#pragma warning restore CS8604 // Possible null reference argument.

                if (foundPost == null)
                {
                    bool deleteSuccess = true;

                    if (
                        _mastodonContext.IsConnected &&
                        postLog.Mastodon_InstanceDomain != null &&
                        postLog.Mastodon_StatusId != null
                    )
                        deleteSuccess = await SyncDeletedPostWithMastodon(postLog);

                    if (
                        _nostrContext.IsConnected &&
                        postLog.Nostr_Author != null &&
                        postLog.Nostr_Id != null
                    )
                        deleteSuccess = true;

                    if (
                        _telegramContext.IsConnected &&
                        postLog.Telegram_ChatId != null &&
                        postLog.Telegram_MessageCount != null &&
                        postLog.Telegram_MessageId != null
                    )
                        deleteSuccess = await SyncDeletedPostWithTelegram(postLog);

                    if (
                        _xContext.IsConnected &&
                        postLog.X_PostId != null
                    )
                        deleteSuccess = await SyncDeletedPostWithX(postLog);

                    if (deleteSuccess)
                    {
                        Say.Success($"Deleted: {postLog.RecordKey}");
                        await PostLogs.DeletePostLog(postLog.RecordKey, _bskyContext.State.Did);
                        Thread.Sleep(syncSleep);
                    }
                }
            }
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    async Task<Embed?> GetEmbedForPost(Post post)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        Embed? embed = null;

        if (post.Record.Embed != null)
        {
            Type embedType = post.Record.Embed.GetType();

            if (
                embedType == typeof(Lib.Polymorphs.AppBsky.EmbedRecord) ||
                post.Record.Embed.GetType() == typeof(Lib.Polymorphs.AppBsky.EmbedRecordWithMedia)
            )
            {
                string recordEmbedCid = "";
                string recordEmbedUri = "";

                switch (embedType)
                {
                    case Type when embedType == typeof(Lib.Polymorphs.AppBsky.EmbedRecord):
                        var recordEmbed = post.Record.Embed as Lib.Polymorphs.AppBsky.EmbedRecord;
                        if (recordEmbed != null)
                        {
                            recordEmbedCid = recordEmbed.Record.Cid;
                            recordEmbedUri = recordEmbed.Record.Uri;
                        }
                        break;
                    case Type when embedType == typeof(Lib.Polymorphs.AppBsky.EmbedRecordWithMedia):
                        var recordEmbedWithMedia = post.Record.Embed as Lib.Polymorphs.AppBsky.EmbedRecordWithMedia;
                        if (recordEmbedWithMedia != null)
                        {
                            var recordEmbedWithMediaRecord = recordEmbedWithMedia.Record as Lib.Polymorphs.AppBsky.EmbedRecord;
                            if (recordEmbedWithMediaRecord != null)
                            {
                                recordEmbedCid = recordEmbedWithMediaRecord.Record.Cid;
                                recordEmbedUri = recordEmbedWithMediaRecord.Record.Uri;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                embed = _bskyHelpers.ParseEmbeds(recordEmbedWithMedia.Media, _bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                            }
                        }
                        break;
                }

                var replyMeta = new Booski.Lib.Internal.AppBsky.Common.ReplyMeta
                {
                    Cid = recordEmbedCid,
                    Uri = recordEmbedUri
                };

                post.Record.Reply = new Booski.Lib.Internal.AppBsky.Common.Reply
                {
                    Parent = replyMeta,
                    Root = replyMeta
                };
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                embed = _bskyHelpers.ParseEmbeds(post.Record.Embed, _bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        return embed;
    }

    async Task<PostLog?> GetReplyParentForPost(Post post)
    {
        PostLog? replyParentPostLog = null;

        if (post.Record.Reply != null)
        {
            var parentReplyKey = post.Record.Reply.Parent.Uri.Split('/').Last();
            var rootReplyKey = post.Record.Reply.Root.Uri.Split('/').Last();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var parentReplyPostLog = await PostLogs.GetPostLog(parentReplyKey, _bskyContext.State.Did);
            var rootReplyPostLog = await PostLogs.GetPostLog(rootReplyKey, _bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (
                parentReplyPostLog != null &&
                rootReplyPostLog != null
            )
                replyParentPostLog = parentReplyPostLog;
        }

        return replyParentPostLog;
    }

    async Task<bool> IsFirstRun()
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        int postLogsCount = await PostLogs.CountPostLogs(_bskyContext.State.Did);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return postLogsCount == 0;
    }

    async Task SyncAddedPostWithMastodon(PostLog postLog, Post post, Embed? embed = null, PostLog? replyParentPostLog = null)
    {
        string? replyToStatusId = null;

        if (replyParentPostLog != null)
            if (replyParentPostLog.Deleted != true)
                replyToStatusId = replyParentPostLog.Mastodon_StatusId;

        try
        {
            var mastodonMessage = await _mastodonHelpers.PostToMastodon(post, embed, replyToStatusId);

            if (mastodonMessage != null)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                postLog = await PostLogs.UpdatePostLog(
                    recordKey: postLog.RecordKey,
                    repository: _bskyContext.State.Did,
                    mastodonInstanceDomain: _mastodonContext.State.Domain,
                    mastodonStatusId: mastodonMessage.Id
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Say.Success($"Posted to {_mastodonContext.State.InstanceSoftware}: {postLog.RecordKey} ({postLog.Mastodon_InstanceDomain}/{postLog.Mastodon_StatusId})");
            }
        }
        catch (Exception e)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Say.Warning($"Unable to post to {_mastodonContext.State.InstanceSoftware}: {post.RecordKey}", e.Message);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }

    async Task SyncAddedPostWithTelegram(PostLog postLog, Post post, Embed? embed = null, PostLog? replyParentPostLog = null)
    {
        int? replyToMessageId = null;

        if (replyParentPostLog != null)
            if (replyParentPostLog.Deleted != true)
                replyToMessageId = replyParentPostLog.Telegram_MessageId;

        try
        {
            var telegramMessage = await _telegramHelpers.PostToTelegram(
                post, embed,
                replyId: replyToMessageId
            );

            if (telegramMessage != null)
            {
                var firstTelegramMessage = telegramMessage.FirstOrDefault();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                postLog = await PostLogs.UpdatePostLog(
                    recordKey: postLog.RecordKey,
                    repository: _bskyContext.State.Did,
                    telegramChatId: firstTelegramMessage.Chat.Id,
                    telegramMessageCount: telegramMessage.Count(),
                    telegramMessageId: firstTelegramMessage.MessageId
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Say.Success($"Posted to Telegram: {postLog.RecordKey} ({postLog.Telegram_ChatId}/{postLog.Telegram_MessageId})");
            }
        }
        catch (Exception e)
        {
            Say.Warning($"Unable to post to Telegram: {post.RecordKey}", e.Message);
        }
    }

    async Task SyncAddedPostWithX(PostLog postLog, Post post, Embed? embed = null, PostLog? replyParentPostLog = null)
    {
        string? replyToPostId = null;

        if (replyParentPostLog != null)
            if (replyParentPostLog.Deleted != true)
                replyToPostId = replyParentPostLog.X_PostId;

        try
        {
            var xMessage = await _xHelpers.PostToX(post, embed, replyToPostId);

            if (
                xMessage != null &&
                xMessage.ID != null
            )
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                postLog = await PostLogs.UpdatePostLog(
                    recordKey: postLog.RecordKey,
                    repository: _bskyContext.State.Did,
                    xPostId: xMessage.ID
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Say.Success($"Posted to X: {postLog.RecordKey} ({postLog.X_PostId})");
            }
        }
        catch (Exception e)
        {
            Say.Warning($"Unable to post to X: {post.RecordKey}", e.Message);
        }
    }

    async Task<bool> SyncDeletedPostWithMastodon(PostLog postLog)
    {
        bool deletedFromMastodon = true;

        string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.Mastodon_InstanceDomain}/{postLog.Mastodon_StatusId})";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if (postLog.Mastodon_InstanceDomain != _mastodonContext.State.Domain)
        {
            deletedFromMastodon = false;
            Say.Warning($"Not deleting from {_mastodonContext.State.InstanceSoftware}: {consoleMessageSuffix}", $"Current instance domain ({_mastodonContext.State.Domain} does not match logged domain ({postLog.Mastodon_InstanceDomain}))");
        }
        else
        {
            try
            {
                if (postLog.Mastodon_StatusId != null)
                    await _mastodonHelpers.DeleteFromMastodon(postLog.Mastodon_StatusId);
                Say.Success($"Deleted from {_mastodonContext.State.InstanceSoftware}: {consoleMessageSuffix}");
            }
            catch (Exception e)
            {
                deletedFromMastodon = false;
                Say.Warning($"Unable to delete from {_mastodonContext.State.InstanceSoftware}: {consoleMessageSuffix}", e.Message);
            }
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return deletedFromMastodon;
    }

    async Task<bool> SyncDeletedPostWithTelegram(PostLog postLog)
    {
        if (postLog == null)
            return false;

        bool deletedFromTelegram = true;
        string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.Telegram_ChatId}/{postLog.Telegram_MessageId})";

#pragma warning disable CS8629 // Nullable value type may be null.
        int currentMessageId = (int)postLog.Telegram_MessageId;
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning disable CS8629 // Nullable value type may be null.
        int lastMessageId = (int)postLog.Telegram_MessageId + (int)postLog.Telegram_MessageCount;
#pragma warning restore CS8629 // Nullable value type may be null.

        while (currentMessageId != lastMessageId)
        {
            try
            {
#pragma warning disable CS8629 // Nullable value type may be null.
                await _telegramHelpers.DeleteFromTelegram(
                    chatId: (long)postLog.Telegram_ChatId,
                    messageId: currentMessageId
                );
#pragma warning restore CS8629 // Nullable value type may be null.
                Say.Success($"Deleted from Telegram: {consoleMessageSuffix}");
            }
            catch (Exception e)
            {
                deletedFromTelegram = false;
                Say.Warning($"Unable to delete from Telegram: {consoleMessageSuffix}", e.Message);
            }

            currentMessageId++;
        }

        return deletedFromTelegram;
    }

    async Task<bool> SyncDeletedPostWithX(PostLog postLog)
    {
        if (postLog == null)
            return false;

        bool deletedFromX = true;
        string consoleMessageSuffix = $"{postLog.RecordKey} ({postLog.X_PostId})";

        try
        {
#pragma warning disable CS8604 // Possible null reference argument.
            await _xHelpers.DeleteFromX(postLog.X_PostId);
#pragma warning restore CS8604 // Possible null reference argument.
            Say.Success($"Deleted from X: {consoleMessageSuffix}");
        }
        catch (Exception e)
        {
            deletedFromX = false;
            Say.Warning($"Unable to delete from X: {consoleMessageSuffix}", e.Message);
        }

        return deletedFromX;
    }

    async Task<PostLog?> UpdatePostLogIgnored(Post post, PostLog? postLog)
    {
        if (
            postLog == null ||
            postLog != null && postLog.Ignored != IgnoredReason.None
        )
            return postLog;

        Embed? embed = await GetEmbedForPost(post);
        PostLog? replyParentPostLog = await GetReplyParentForPost(post);
        IgnoredReason? ignoredReason = null;

        if (postLog != null)
        {
            // ReplyButNoParent (2)
            if (
                post.Record.Reply != null && replyParentPostLog == null ||
                post.Record.Reply != null && replyParentPostLog != null && replyParentPostLog.Ignored != IgnoredReason.None
            )
            {
                // BUG: If you start Booski after a long period and an un-synced post is a reply
                //      to an un-synced parent it will be inadvertently ignored.
                //      --retry-ignored can be passed by the user to attempt to repair these.
                Say.Warning($"Ignoring: {postLog.RecordKey}", "Post is a reply, but parent doesn't exist (either deleted, ignored or not ours)");
                ignoredReason = IgnoredReason.ReplyButNoParent;
            }

            // EmbedsButNotSupported (3)
            if (
                embed != null &&
                embed.Items.Count() == 0 &&
                embed.Type != EmbedType.Unknown
            )
            {
                Say.Warning($"Ignoring: {postLog.RecordKey}", "Post has embeds but none are supported");
                ignoredReason = IgnoredReason.EmbedsButNotSupported;
            }

            // OldCreatedDate (5)
            if (post.Record.CreatedAt.AddHours(OldPostHours) < DateTime.UtcNow)
            {
                var hoursUnit = "hours";
                if (OldPostHours == 1)
                    hoursUnit = "hour";

                Say.Warning($"Ignoring: {postLog.RecordKey}", $"Post is older than {OldPostHours} {hoursUnit}");
                ignoredReason = IgnoredReason.OldCreatedAtDate;
            }

            // StartsWithMention (4)
            if (post.Record.Text.StartsWith("@")) // TODO: Check if this is a real mention?
            {
                Say.Warning($"Ignoring: {postLog.RecordKey}", "Post starts with \"@\"");
                ignoredReason = IgnoredReason.StartsWithMention;
            }

            if (ignoredReason != null)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                postLog = await PostLogs.IgnorePostLog(postLog.RecordKey, _bskyContext.State.Did, (IgnoredReason)ignoredReason);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        return postLog;
    }
}