using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    public class FeedThreadViewPost : Polymorph {
        public Post Post { get; set; }
        public Polymorph Parent { get; set; }
        public List<Polymorph> Replies { get; set; }
    }
}