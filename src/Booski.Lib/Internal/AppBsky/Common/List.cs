
namespace Booski.Lib.Internal.AppBsky.Common {
    public class List {
        public Uri Avatar { get; set; }
        public string Cid { get; set; }
        public DateTime IndexedAt { get; set; }
        public List<Label> Labels { get; set; }
        public string Name { get; set; }
        public dynamic Purpose { get; set; } // NOTE: JSON polymorphism is busted in .NET 8.0 (and below): https://github.com/dotnet/runtime/issues/72604#issuecomment-1440708052
        public Uri Uri { get; set; }
        public Viewer Viewer { get; set; }
    }   
}