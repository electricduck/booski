using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Enums;
using Booski.Lib.Internal.ComAtproto.Enums;
using Booski.Lib.Internal.AppBsky.Responses;
using Booski.Lib.Internal.ComAtproto.Responses;
using Booski.Lib.Internal.XrpcBase.Responses;
using AppBskyCommon = Booski.Lib.Internal.AppBsky.Common;
using AppBskyConstants = Booski.Lib.Internal.AppBsky.Common.Constants;
using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;
using ComAtprotoCommon =  Booski.Lib.Internal.ComAtproto.Common;
using ComAtprotoConstants = Booski.Lib.Internal.ComAtproto.Common.Constants;

namespace Booski.Lib.Xrpc {
    public class App {
        public class Bsky {
            public interface Actor {
                public Task<AtProtoApiResponse<GetPreferencesResponse>> GetPreferences();
                public Task<AtProtoApiResponse<AppBskyLexicons.Actor.Defs_ProfileViewDetailed>> GetProfile(string actor);
                public Task<AtProtoApiResponse<GetProfilesResponse>> GetProfiles(string[] actors);
                public Task<AtProtoApiResponse<GetSuggestionsResponse>> GetSuggestions(int limit = AppBskyConstants.Defaults.ResultsLimit, string cursor = "");
                public Task<AtProtoApiResponse<PutPreferencesResponse>> PutPreferences(List<Lexicon> preferences);
                public Task<AtProtoApiResponse<SearchActorsResponse>> SearchActors(string query = "", int limit = 10, string cursor = "");
                public Task<AtProtoApiResponse<SearchActorsTypeaheadResponse>> SearchActorsTypeahead(string query = "", int limit = 10);
            }

            public interface Feed {
                public Task<AtProtoApiResponse<DescribeFeedGeneratorResponse>> DescribeFeedGenerator();
                public Task<AtProtoApiResponse<GetActorFeedsResponse>> GetActorFeeds(string actor = "", string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetActorLikesResponse>> GetActorLikes(string actor = "", string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetAuthorFeedResponse>> GetAuthorFeed(string actor = "", string cursor = "", AuthorFeedFilter filter = AuthorFeedFilter.PostsWithReplies, int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetFeedResponse>> GetFeed(string feed, string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetFeedGeneratorResponse>> GetFeedGenerator(string feed);
                public Task<AtProtoApiResponse<GetFeedGeneratorsResponse>> GetFeedGenerators(string[] feeds);
                public Task<AtProtoApiResponse<GetFeedSkeletonResponse>> GetFeedSkeleton(string feed, string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetLikesResponse>> GetLikes(string uri, string cid = "", string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetListFeedResponse>> GetListFeed(string list, string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetPostThreadResponse>> GetPostThread(string uri, int depth = 6, int parentHeight = 80);
                public Task<AtProtoApiResponse<GetPostsResponse>> GetPosts(string[] uris);
                public Task<AtProtoApiResponse<GetRepostedByResponse>> GetRepostedBy(string uri, string cid = "", string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetSuggestedFeedsResponse>> GetSuggestedFeeds(int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<GetTimelineResponse>> GetTimeline(string algorithm = "", string cursor = "", int limit = AppBskyConstants.Defaults.ResultsLimit);
                public Task<AtProtoApiResponse<SearchPostsResponse>> SearchPosts(string query, string author = "", string cursor = "", string domain = "", string mentions = "", string language = "", int limit = AppBskyConstants.Defaults.SearchResultsLimit, DateTime? since = null, SearchSort sort = AppBskyConstants.Defaults.SearchSortDefault, string[] tag = null, DateTime? until = null, string url = "");
                public Task<AtProtoApiResponse<dynamic>> SendInteractions(List<AppBskyCommon.Interaction> interactions);
            }
        }
    }

    public class Com {
        public class Atproto {
            public interface Admin {
                public Task<AtProtoApiResponse<GetInviteCodesResponse>> GetInviteCodes(int limit = ComAtprotoConstants.DefaultInviteCodeLimit, InviteCodeSort sort = ComAtprotoConstants.DefaultInviteCodeSort);
            }

            public interface Identity {
                public Task<AtProtoApiResponse<ResolveHandleResponse>> ResolveHandle(string did);
            }

            public interface Repo {
                public Task<AtProtoApiResponse<ApplyWritesResponse>> ApplyWrites(string repo, List<Lexicon> writes, string swapCommit = "", bool validate = true);
                public Task<AtProtoApiResponse<CreateRecordResponse>> CreateRecord(string collection, dynamic record, string repo, string rKey = "", string swapCommit = "", bool validate = true);
                public Task<AtProtoApiResponse<DeleteRecordResponse>> DeleteRecord(string collection, string repo, string rKey = "", string swapCommit = "", string swapRecord = "");
                public Task<AtProtoApiResponse<DescribeRepoResponse>> DescribeRepo(string repo);
                public Task<AtProtoApiResponse<GetRecordResponse>> GetRecord(string collection, string repo, string rKey, string cid = "");
                public Task<AtProtoApiResponse<ImportRepoResponse>> ImportRepo(byte[] blob);
                public Task<AtProtoApiResponse<ImportRepoResponse>> ImportRepo(string path);
                public Task<AtProtoApiResponse<ListMissingBlobsReponse>> ListMissingBlobs(string cursor = "", int limit = 500);
                public Task<AtProtoApiResponse<ListRecordsResponse>> ListRecords(string collection, string repo, string cursor = "", int limit = 50, bool reverse = false);
                public Task<AtProtoApiResponse<PutRecordResponse>> PutRecord(string collection, dynamic record, string repo, string rKey, string swapCommit = "", string swapRecord = "", bool validate = true);
                public Task<AtProtoApiResponse<UploadBlobResponse>> UploadBlob(byte[] blob);
                public Task<AtProtoApiResponse<UploadBlobResponse>> UploadBlob(string path);
            }

            public interface Server {
                public Task<AtProtoApiResponse<CheckAccountStatusResponse>> CheckAccountStatus();
                public Task<AtProtoApiResponse<CreateInviteCodeResponse>> CreateInviteCode(int useCount, string forAccount = "");
                public Task<AtProtoApiResponse<CreateSessionResponse>> CreateSession(string identifier, string password, string? authFactorToken = "");
                public Task<AtProtoApiResponse<DescribeServerResponse>> DescribeServer();
                public Task<AtProtoApiResponse<GetSessionResponse>> GetSession();
                public Task<AtProtoApiResponse<RefreshSessionResponse>> RefreshSession();
            }

            public interface Sync {
                public Task<AtProtoApiResponse<GetLatestCommitResponse>> GetLatestCommit(string did);
            }
        }
    }

    public interface _ {
        Task<AtProtoApiResponse<GetHealthResponse>> GetHealth();
    }
}