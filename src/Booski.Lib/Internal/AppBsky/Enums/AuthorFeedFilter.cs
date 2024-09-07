using System.Runtime.Serialization;

namespace Booski.Lib.Internal.AppBsky.Enums {
    public enum AuthorFeedFilter {
        Unknown = 0,
        [EnumMember(Value = "posts_and_author_threads")]
        PostsAndAuthorThreads,
        [EnumMember(Value = "posts_no_replies")]
        PostsNoReplies,
        [EnumMember(Value = "posts_with_media")]
        PostsWithMedia,
        [EnumMember(Value = "posts_with_replies")]
        PostsWithReplies
    }
}