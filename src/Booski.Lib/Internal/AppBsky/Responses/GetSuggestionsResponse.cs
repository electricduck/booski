using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class GetSuggestionsResponse {
        public List<Actor> Actors { get; set; }
        public string Cursor { get; set; }
    }
}