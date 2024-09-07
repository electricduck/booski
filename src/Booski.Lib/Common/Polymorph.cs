using System.Text.Json.Serialization;
using AppBskyPolymorphs = Booski.Lib.Polymorphs.AppBsky;
using ComAtprotoPolymorphs = Booski.Lib.Polymorphs.ComAtproto;

namespace Booski.Lib.Common {
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorAdultContentPref), typeDiscriminator: "app.bsky.actor.defs#adultContentPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorContentLabelPref), typeDiscriminator: "app.bsky.actor.defs#contentLabelPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorFeedViewPref), typeDiscriminator: "app.bsky.actor.defs#feedViewPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorHiddenPostsPref), typeDiscriminator: "app.bsky.actor.defs#hiddenPostsPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorInterestsPref), typeDiscriminator: "app.bsky.actor.defs#interestsPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorMutedWordsPref), typeDiscriminator: "app.bsky.actor.defs#mutedWordsPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorPersonalDetailsPref), typeDiscriminator: "app.bsky.actor.defs#personalDetailsPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorSavedFeedsPrefV2), typeDiscriminator: "app.bsky.actor.defs#savedFeedsPrefV2")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorSavedFeedsPref), typeDiscriminator: "app.bsky.actor.defs#savedFeedsPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.ActorThreadViewPref), typeDiscriminator: "app.bsky.actor.defs#threadViewPref")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedExternal), typeDiscriminator: "app.bsky.embed.external")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedExternalView), typeDiscriminator: "app.bsky.embed.external#view")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedImages), typeDiscriminator: "app.bsky.embed.images")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedImagesView), typeDiscriminator: "app.bsky.embed.images#view")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedRecord), typeDiscriminator: "app.bsky.embed.record")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedRecordView), typeDiscriminator: "app.bsky.embed.record#view")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedRecordWithMedia), typeDiscriminator: "app.bsky.embed.recordWithMedia")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.EmbedRecordWithMediaView), typeDiscriminator: "app.bsky.embed.recordWithMedia#view")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedBlockedPost), typeDiscriminator: "app.bsky.feed.defs#blockedPost")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedNotFoundPost), typeDiscriminator: "app.bsky.feed.defs#notFoundPost")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedPostView), typeDiscriminator: "app.bsky.feed.defs#postView")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedReasonReport), typeDiscriminator: "app.bsky.feed.defs#reasonRepost")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedSkeletonReasonRepost), typeDiscriminator: "app.bsky.feed.defs#skeletonReasonRepost")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedThreadViewPost), typeDiscriminator: "app.bsky.feed.defs#threadViewPost")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.FeedPost), typeDiscriminator: "app.bsky.feed.post")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.RichtextFacetLink), typeDiscriminator: "app.bsky.richtext.facet#link")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.RichtextFacetMention), typeDiscriminator: "app.bsky.richtext.facet#mention")]
    [JsonDerivedType(typeof(AppBskyPolymorphs.RichtextFacetTag), typeDiscriminator: "app.bsky.richtext.facet#tag")]
    [JsonDerivedType(typeof(ComAtprotoPolymorphs.LabelDefsSelfLabels), typeDiscriminator: "com.atproto.label.defs#selfLabels")]
    [JsonDerivedType(typeof(ComAtprotoPolymorphs.RepoApplyWritesCreate), typeDiscriminator: "com.atproto.repo.applyWrites.create")]
    [JsonDerivedType(typeof(ComAtprotoPolymorphs.RepoApplyWritesDelete), typeDiscriminator: "com.atproto.repo.applyWrites.delete")]
    [JsonDerivedType(typeof(ComAtprotoPolymorphs.RepoApplyWritesUpdate), typeDiscriminator: "com.atproto.repo.applyWrites.update")]
    public class Polymorph {}
}