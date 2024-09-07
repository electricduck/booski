using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class SearchPostsResponse {
        public string Cursor { get; set; }
        public int HitsTotal { get; set; }
        public List<Post> Posts { get; set; }
    }
}