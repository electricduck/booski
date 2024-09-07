using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetAuthorFeedResponse {
        public string Cursor { get; set; }
        public List<Feeds> Feed { get; set; }
    }
}