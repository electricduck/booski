using System.Runtime.Serialization;

namespace Booski.Lib.Internal.ComAtproto.Enums {
    public enum InviteCodeSort {
        Unknown = 0,
        [EnumMember(Value = "recent")]
        Recent,
        [EnumMember(Value = "usage")]
        Usage
    }
}