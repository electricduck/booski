using System.Text.Json.Serialization;
using Booski.Lib.Common;
using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Lexicons.AppBsky {
    // app.bsky.feed.post
    public class FeedPost : Lexicon {
        public DateTime CreatedAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Lexicon Embed { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Facet> Facets { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Langs { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Lexicon Labels { get; set; }
        public Reply Reply { get; set; }
        public string Text { get; set; }
    }
}