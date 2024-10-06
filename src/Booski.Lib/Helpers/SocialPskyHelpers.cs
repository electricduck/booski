using SocialPskyLexicons = Booski.Lib.Lexicons.SocialPsky;

namespace Booski.Lib.Helpers {
    public static class SocialPskyHelpers {
        public static SocialPskyLexicons.Feed.Post CreateFeedPost(
            string text
        )
        {
            SocialPskyLexicons.Feed.Post feedPost = new SocialPskyLexicons.Feed.Post();

            feedPost.Text = text;

            return feedPost;
        }
    }
}