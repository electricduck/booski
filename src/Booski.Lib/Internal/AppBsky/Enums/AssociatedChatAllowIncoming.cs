using System.Runtime.Serialization;

namespace Booski.Lib.Internal.AppBsky.Enums {
    public enum AssociatedChatAllowIncoming {
        Unknown = 0,
        [EnumMember(Value = "all")]
        All,
        [EnumMember(Value = "following")]
        Following,
        [EnumMember(Value = "none")]
        None
    }
}