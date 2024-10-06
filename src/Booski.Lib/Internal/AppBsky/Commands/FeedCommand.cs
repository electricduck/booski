using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;
using Booski.Lib.Internal.AppBsky.Enums;
using Booski.Lib.Internal.AppBsky.Requests;
using Booski.Lib.Internal.AppBsky.Responses;
using Booski.Lib.Xrpc;
using Booski.Lib.Utilities;
using AppBskyConstants = Booski.Lib.Internal.AppBsky.Common.Constants;

namespace Booski.Lib.Internal.AppBsky.Commands {
    public class FeedCommand : App.Bsky.Feed
    {
        private readonly AtProto _atProto;

        public FeedCommand(
            AtProto atProto
        ) {
            _atProto = atProto;
        }

        public async Task<AtProtoApiResponse<DescribeFeedGeneratorResponse>> DescribeFeedGenerator()
        {
            return await _atProto.GetJson<DescribeFeedGeneratorResponse>(
                lexicon: AppBskyConstants.Routes.FeedDescribeFeedGenerator
            );
        }

        public async Task<AtProtoApiResponse<GetActorFeedsResponse>> GetActorFeeds(
            string actor = "",
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        )
        {
            List<QueryParam> getActorFeedsParams = new List<QueryParam> {
                new QueryParam("actor", [actor]),
                new QueryParam("cursor", [cursor]),
                new QueryParam("limit", [limit.ToString()])
            };

            return await _atProto.GetJson<GetActorFeedsResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetActorFeeds,
                queries: getActorFeedsParams
            );
        }

         public async Task<AtProtoApiResponse<GetActorLikesResponse>> GetActorLikes(
            string actor = "",
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        )
        {
            List<QueryParam> getActorLikesParams = new List<QueryParam> {
                new QueryParam("actor", [actor]),
                new QueryParam("cursor", [cursor]),
                new QueryParam("limit", [limit.ToString()])
            };

            return await _atProto.GetJson<GetActorLikesResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetActorLikes,
                queries: getActorLikesParams
            );
        }

        public async Task<AtProtoApiResponse<GetAuthorFeedResponse>> GetAuthorFeed(
            string actor = "",
            string cursor = "",
            AuthorFeedFilter filter = AuthorFeedFilter.PostsWithReplies,
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            var filterString = EnumUtilities.GetEnumMemberValue<AuthorFeedFilter>(filter);

            List<QueryParam> getAuthorFeedParams = new List<QueryParam> {
                new QueryParam("actor", actor),
                new QueryParam("cursor", cursor),
                new QueryParam("filter", filterString),
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<GetAuthorFeedResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetAuthorFeed,
                queries: getAuthorFeedParams
            );
        }

        public async Task<AtProtoApiResponse<GetFeedResponse>> GetFeed(
            string feed,
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getFeedParams = new List<QueryParam> {
                new QueryParam("cursor", cursor),
                new QueryParam("feed", feed),
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<GetFeedResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetFeed,
                queries: getFeedParams
            );
        }

        public async Task<AtProtoApiResponse<GetFeedGeneratorResponse>> GetFeedGenerator(
            string feed
        ) {
            List<QueryParam> getFeedGeneratorParams = new List<QueryParam> {
                new QueryParam("feed", feed)
            };

            return await _atProto.GetJson<GetFeedGeneratorResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetFeedGenerator,
                queries: getFeedGeneratorParams
            );
        }

        public async Task<AtProtoApiResponse<GetFeedGeneratorsResponse>> GetFeedGenerators(
            string[] feeds
        ) {
            List<QueryParam> getFeedGeneratorsParams = new List<QueryParam> {
                new QueryParam("feed", feeds)
            };

            return await _atProto.GetJson<GetFeedGeneratorsResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetFeedGenerators,
                queries: getFeedGeneratorsParams
            );
        }

        public async Task<AtProtoApiResponse<GetFeedSkeletonResponse>> GetFeedSkeleton(
            string feed,
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getFeedSkeletonParams = new List<QueryParam> {
                new QueryParam("cursor", cursor),
                new QueryParam("feed", feed),
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<GetFeedSkeletonResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetFeedSkeleton,
                queries: getFeedSkeletonParams
            );
        }

        public async Task<AtProtoApiResponse<GetLikesResponse>> GetLikes(
            string uri,
            string cid = "",
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getLikesParams = new List<QueryParam> {
                new QueryParam("cid", cid),
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString()),
                new QueryParam("uri", uri)
            };

            return await _atProto.GetJson<GetLikesResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetLikes,
                queries: getLikesParams
            );
        }

        public async Task<AtProtoApiResponse<GetListFeedResponse>> GetListFeed(
            string list,
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getListFeedParams = new List<QueryParam> {
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString()),
                new QueryParam("list", list)
            };

            return await _atProto.GetJson<GetListFeedResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetListFeed,
                queries: getListFeedParams
            );
        }

        public async Task<AtProtoApiResponse<GetPostThreadResponse>> GetPostThread(
            string uri,
            int depth = 6,
            int parentHeight = 80
        ) {
            List<QueryParam> getPostThreadParams = new List<QueryParam> {
                new QueryParam("depth", depth.ToString()),
                new QueryParam("parentHeight", parentHeight.ToString()),
                new QueryParam("uri", uri)
            };

            return await _atProto.GetJson<GetPostThreadResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetPostThread,
                queries: getPostThreadParams
            );
        }

        public async Task<AtProtoApiResponse<GetPostsResponse>> GetPosts(
            string[] uris
        ) {
            List<QueryParam> getPostsParams = new List<QueryParam> {
                new QueryParam("uris", uris)
            };

            return await _atProto.GetJson<GetPostsResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetPosts,
                queries: getPostsParams
            );
        }

        public async Task<AtProtoApiResponse<GetRepostedByResponse>> GetRepostedBy(
            string uri,
            string cid = "",
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getRepostedByParams = new List<QueryParam> {
                new QueryParam("cid", cid),
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString()),
                new QueryParam("uri", uri)
            };

            return await _atProto.GetJson<GetRepostedByResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetRepostedBy,
                queries: getRepostedByParams
            );
        }

        public async Task<AtProtoApiResponse<GetSuggestedFeedsResponse>> GetSuggestedFeeds(
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getSuggestedFeedsParams = new List<QueryParam> {
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<GetSuggestedFeedsResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetSuggestedFeeds,
                queries: getSuggestedFeedsParams
            );
        }

        public async Task<AtProtoApiResponse<GetTimelineResponse>> GetTimeline(
            string algorithm = "",
            string cursor = "",
            int limit = AppBskyConstants.Defaults.ResultsLimit
        ) {
            List<QueryParam> getTimelineParams = new List<QueryParam> {
                new QueryParam("algorithm", algorithm),
                new QueryParam("cursor", cursor),
                new QueryParam("limit", limit.ToString())
            };

            return await _atProto.GetJson<GetTimelineResponse>(
                lexicon: AppBskyConstants.Routes.FeedGetTimeline,
                queries: getTimelineParams
            );
        }

        public async Task<AtProtoApiResponse<SearchPostsResponse>> SearchPosts(
            string query,
            string author = "",
            string cursor = "",
            string domain = "",
            string mentions = "",
            string language = "",
            int limit = AppBskyConstants.Defaults.SearchResultsLimit,
            DateTime? since = null,
            SearchSort sort = AppBskyConstants.Defaults.SearchSortDefault,
            string[] tag = null,
            DateTime? until = null,
            string url = ""
        ) {
            string sinceString = "";
            string sortString = EnumUtilities.GetEnumMemberValue<SearchSort>(sort);
            string untilString = "";

            if(since != null)
                sinceString = DateUtilities.GetJsDateTime((DateTime)since);

            if(until != null)
                untilString = DateUtilities.GetJsDateTime((DateTime)until);

            List<QueryParam> searchPostsParams = new List<QueryParam> {
                new QueryParam("author", author),
                new QueryParam("cursor", cursor),
                new QueryParam("domain", domain),
                new QueryParam("lang", language),
                new QueryParam("limit", limit.ToString()),
                new QueryParam("mentions", mentions),
                new QueryParam("q", query),
                new QueryParam("since", sinceString),
                new QueryParam("sort", sortString),
                new QueryParam("tag", tag),
                new QueryParam("until", untilString),
                new QueryParam("url", url)
            };

            return await _atProto.GetJson<SearchPostsResponse>(
                lexicon: AppBskyConstants.Routes.FeedSearchPosts,
                queries: searchPostsParams
            );
        }

        // NOTE: Incomplete API docs (https://docs.bsky.app/docs/api/app-bsky-feed-send-interactions). What does this do?
        public async Task<AtProtoApiResponse<dynamic>> SendInteractions(
            List<Interaction> interactions
        ) {
            SendInteractionsRequest sendInteractionsRequest = new SendInteractionsRequest {
                Interactions = interactions
            };

            return await _atProto.PostJson<dynamic, SendInteractionsRequest>(
                lexicon: AppBskyConstants.Routes.FeedSendInteractions,
                body: sendInteractionsRequest
            );
        }
    }
}