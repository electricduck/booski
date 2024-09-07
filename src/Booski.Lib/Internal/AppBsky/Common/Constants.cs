using Booski.Lib.Internal.AppBsky.Enums;

namespace Booski.Lib.Internal.AppBsky.Common {
    public static class Constants {
        public static class Defaults {
            public const int ResultsLimit = 50;
            public const int SearchResultsLimit = 25;
            public const SearchSort SearchSortDefault = SearchSort.Latest;
        }

        public static class Routes {
            public static class Base {
                public const string Actor = $"{Prefix}.actor";
                public const string Feed = $"{Prefix}.feed";
                public const string Graph = $"{Prefix}.graph";
                public const string Labeler = $"{Prefix}.labeler";
                public const string Notification = $"{Prefix}.notification";
                public const string Prefix = "app.bsky";
            }

            public const string ActorGetProfile = $"{Base.Actor}.getProfile";
            public const string ActorGetProfiles = $"{Base.Actor}.getProfiles";
            public const string ActorGetPreferences = $"{Base.Actor}.getPreferences";
            public const string ActorGetSuggestions = $"{Base.Actor}.getSuggestions";
            public const string ActorPutPreferences = $"{Base.Actor}.putPreferences";
            public const string ActorSearchActors = $"{Base.Actor}.searchActors";
            public const string ActorSearchActorsTypeahead = $"{Base.Actor}.searchActorsTypeahead";
            public const string FeedDescribeFeedGenerator = $"{Base.Feed}.describeFeedGenerator";
            public const string FeedGetActorFeeds = $"{Base.Feed}.getActorFeeds";
            public const string FeedGetActorLikes = $"{Base.Feed}.getActorLikes";
            public const string FeedGetAuthorFeed = $"{Base.Feed}.getAuthorFeed";
            public const string FeedGetFeed = $"{Base.Feed}.getFeed";
            public const string FeedGetFeedGenerator = $"{Base.Feed}.getFeedGenerator";
            public const string FeedGetFeedGenerators = $"{Base.Feed}.getFeedGenerators";
            public const string FeedGetFeedSkeleton = $"{Base.Feed}.getFeedSkeleton";
            public const string FeedGetLikes = $"{Base.Feed}.getLikes";
            public const string FeedGetListFeed = $"{Base.Feed}.getListFeed";
            public const string FeedGetPostThread = $"{Base.Feed}.getPostThread";
            public const string FeedGetPosts = $"{Base.Feed}.getPosts";
            public const string FeedGetRepostedBy = $"{Base.Feed}.getRepostedBy";
            public const string FeedGetSuggestedFeeds = $"{Base.Feed}.getSuggestedFeeds";
            public const string FeedGetTimeline = $"{Base.Feed}.getTimeline";
            public const string FeedSearchPosts = $"{Base.Feed}.searchPosts";
            public const string FeedSendInteractions = $"{Base.Feed}.sendInteractions";
        }
    }
}