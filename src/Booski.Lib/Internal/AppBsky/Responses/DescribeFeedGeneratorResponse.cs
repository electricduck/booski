using Booski.Lib.Internal.AppBsky.Common;

namespace Booski.Lib.Internal.AppBsky.Responses {
    public class DescribeFeedGeneratorResponse {
        public string Did { get; set; }
        public List<FeedUri> Feeds { get; set; }
        public Links Links { get; set; }
    }
}