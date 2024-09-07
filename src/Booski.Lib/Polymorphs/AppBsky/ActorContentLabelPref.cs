using Booski.Lib.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.actor.defs#contentLabelPref
    public class ActorContentLabelPref : Polymorph {
        public string Label { get; set; }
        public string LabelerDid { get; set; }
        public string Visibility { get; set; }
    }
}