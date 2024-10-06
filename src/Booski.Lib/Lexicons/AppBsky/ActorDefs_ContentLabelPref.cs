using Booski.Lib.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.actor.defs#contentLabelPref
    public class ActorDefs_ContentLabelPref : Lexicon {
        public string Label { get; set; }
        public string LabelerDid { get; set; }
        public string Visibility { get; set; }
    }
}