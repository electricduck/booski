
namespace Booski.Lib.Internal.AppBsky.Common {
    public class Feed {
        public bool AcceptsInteractions { get; set; }
        public string Avatar { get; set; }
        public string Cid { get; set; }
        public Actor Creator { get; set; }
        public string Description { get; set; }
        public List<Facet> DescriptionFacets { get; set; } // NOTE: JSON polymorphism is busted in .NET 8.0 (and below): https://github.com/dotnet/runtime/issues/72604#issuecomment-1440708052
        public string Did { get; set; }
        public string DisplayName { get; set; }
        public DateTime IndexedAt { get; set; }
        public List<Label> Labels { get; set; }
        public int LikeCount { get; set; }
        public string Uri { get; set; }
        public Viewer Viewer { get; set; }
    }
}