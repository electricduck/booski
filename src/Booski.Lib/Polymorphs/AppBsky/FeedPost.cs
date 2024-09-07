using System.Text.Json.Serialization;
using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Polymorphs.AppBsky {
    // app.bsky.feed.post
    public class FeedPost : Polymorph {
        public DateTime CreatedAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Polymorph Embed { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Facet> Facets { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Langs { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Polymorph Labels { get; set; }
        public Reply Reply { get; set; }
        public string Text { get; set; }
    }
}