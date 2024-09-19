
namespace Booski.Enums {
    public enum IgnoredReason {
        None = 0,
        Manual = Int32.MaxValue,
        EmbedsButNotSupported = 3,
        FirstRun = 1,
        OldCreatedAtDate = 5,
        ReplyButNoParent = 2,
        StartsWithMention = 4
    }
}