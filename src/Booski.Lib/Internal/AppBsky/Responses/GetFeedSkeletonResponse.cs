using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetFeedSkeletonResponse {
        public string Cursor { get; set; }
        public List<FeedSkeleton> Feed { get; set; }
    }
}