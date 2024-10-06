using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    public class FeedDefs_ThreadViewPost : Lexicon {
        public Post Post { get; set; }
        public Lexicon Parent { get; set; }
        public List<Lexicon> Replies { get; set; }
    }
}