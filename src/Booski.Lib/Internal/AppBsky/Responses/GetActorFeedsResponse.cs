using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetActorFeedsResponse {
        public string Cursor { get; set; }
        public List<Feed> Feeds { get; set; }
    }
}