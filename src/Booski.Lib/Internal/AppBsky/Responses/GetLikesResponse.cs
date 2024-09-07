using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetLikesResponse {
        public string Cid { get; set; }
        public string Cursor { get; set; }
        public List<Like> Likes { get; set; }
        public string Uri { get; set; }
    }
}