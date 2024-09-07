using System.Text.Json.Serialization;

namespace Booski.Lib.Internal.ComAtproto.Common {
    [JsonDerivedType(typeof(FileBlob), typeDiscriminator: "blob")]
    public class File {
    }
}