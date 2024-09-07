using System.Runtime.Serialization;

namespace Booski.Lib.Internal.AppBsky.Enums {
    public enum SearchSort {
        Unknown = 0,
        [EnumMember(Value = "latest")]
        Latest,
        [EnumMember(Value = "top")]
        Top
    }
}