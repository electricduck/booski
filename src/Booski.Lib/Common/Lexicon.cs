using System.Text.Json.Serialization;
using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;
using ComAtprotoLexicons = Booski.Lib.Lexicons.ComAtproto;
using SocialPskyLexicons = Booski.Lib.Lexicons.SocialPsky;

namespace Booski.Lib.Common {
    // ----------------
    // -- app.bsky.* --
    // ----------------

    // app.bsky.*
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_AdultContentPref), typeDiscriminator: "app.bsky.actor.defs#adultContentPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_ContentLabelPref), typeDiscriminator: "app.bsky.actor.defs#contentLabelPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_FeedViewPref), typeDiscriminator: "app.bsky.actor.defs#feedViewPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_HiddenPostsPref), typeDiscriminator: "app.bsky.actor.defs#hiddenPostsPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_InterestsPref), typeDiscriminator: "app.bsky.actor.defs#interestsPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_MutedWordsPref), typeDiscriminator: "app.bsky.actor.defs#mutedWordsPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_PersonalDetailsPref), typeDiscriminator: "app.bsky.actor.defs#personalDetailsPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_SavedFeedsPrefV2), typeDiscriminator: "app.bsky.actor.defs#savedFeedsPrefV2")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_SavedFeedsPref), typeDiscriminator: "app.bsky.actor.defs#savedFeedsPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.ActorDefs_ThreadViewPref), typeDiscriminator: "app.bsky.actor.defs#threadViewPref")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedExternal), typeDiscriminator: "app.bsky.embed.external")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedExternal_View), typeDiscriminator: "app.bsky.embed.external#view")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedImages), typeDiscriminator: "app.bsky.embed.images")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedImages_View), typeDiscriminator: "app.bsky.embed.images#view")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedRecord), typeDiscriminator: "app.bsky.embed.record")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedRecord_View), typeDiscriminator: "app.bsky.embed.record#view")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedRecordWithMedia), typeDiscriminator: "app.bsky.embed.recordWithMedia")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedRecordWithMedia_View), typeDiscriminator: "app.bsky.embed.recordWithMedia#view")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedVideo), typeDiscriminator: "app.bsky.embed.video")]
    [JsonDerivedType(typeof(AppBskyLexicons.EmbedVideo_View), typeDiscriminator: "app.bsky.embed.video#view")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_BlockedPost), typeDiscriminator: "app.bsky.feed.defs#blockedPost")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_NotFoundPost), typeDiscriminator: "app.bsky.feed.defs#notFoundPost")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_PostView), typeDiscriminator: "app.bsky.feed.defs#postView")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_ReasonReport), typeDiscriminator: "app.bsky.feed.defs#reasonRepost")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_SkeletonReasonRepost), typeDiscriminator: "app.bsky.feed.defs#skeletonReasonRepost")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedDefs_ThreadViewPost), typeDiscriminator: "app.bsky.feed.defs#threadViewPost")]
    [JsonDerivedType(typeof(AppBskyLexicons.FeedPost), typeDiscriminator: "app.bsky.feed.post")]
    [JsonDerivedType(typeof(AppBskyLexicons.RichtextFacet_Link), typeDiscriminator: "app.bsky.richtext.facet#link")]
    [JsonDerivedType(typeof(AppBskyLexicons.RichtextFacet_Mention), typeDiscriminator: "app.bsky.richtext.facet#mention")]
    [JsonDerivedType(typeof(AppBskyLexicons.RichtextFacet_Tag), typeDiscriminator: "app.bsky.richtext.facet#tag")]

    // app.bsky.profile.*
    [JsonDerivedType(typeof(AppBskyLexicons.Actor.Defs_ProfileViewDetailed), typeDiscriminator: "app.bsky.actor.defs#profileViewDetailed")]
    [JsonDerivedType(typeof(AppBskyLexicons.Actor.Profile), typeDiscriminator: "app.bsky.actor.profile")]

    // -------------------
    // -- com.atproto.* --
    // -------------------

    // com.atproto.*
    [JsonDerivedType(typeof(ComAtprotoLexicons.LabelDefs_SelfLabels), typeDiscriminator: "com.atproto.label.defs#selfLabels")]
    [JsonDerivedType(typeof(ComAtprotoLexicons.RepoApplyWritesCreate), typeDiscriminator: "com.atproto.repo.applyWrites.create")]
    [JsonDerivedType(typeof(ComAtprotoLexicons.RepoApplyWritesDelete), typeDiscriminator: "com.atproto.repo.applyWrites.delete")]
    [JsonDerivedType(typeof(ComAtprotoLexicons.RepoApplyWritesUpdate), typeDiscriminator: "com.atproto.repo.applyWrites.update")]

    // com.atproto.repo.*
    [JsonDerivedType(typeof(ComAtprotoLexicons.Repo.StrongRef), typeDiscriminator: "com.atproto.repo.strongRef")]

    // -------------------
    // -- social.psky.* --
    // -------------------

    // social.psky.actor.*
    [JsonDerivedType(typeof(SocialPskyLexicons.Actor.Profile), typeDiscriminator: "social.psky.actor.profile")]

    // social.psky.feed.*
    [JsonDerivedType(typeof(SocialPskyLexicons.Feed.Post), typeDiscriminator: "social.psky.feed.post")]

    public class Lexicon {}
}