
namespace Booski.Lib.Internal.AppBsky.Common {
    public class Threadgate {
        public string Cid { get; set; }
        public List<List> Lists { get; set; }
        public dynamic Record { get; set; } // NOTE: JSON polymorphism is busted in .NET 8.0 (and below): https://github.com/dotnet/runtime/issues/72604#issuecomment-1440708052
        public string Uri { get; set; }
    }
}