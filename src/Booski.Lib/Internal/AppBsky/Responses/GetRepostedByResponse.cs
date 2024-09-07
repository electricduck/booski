using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetRepostedByResponse {
        public string Cid { get; set; }
        public string Cursor { get; set; }
        public List<Actor> RepostedBy { get; set; }
        public string Uri { get; set; }
    }
}