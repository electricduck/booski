using Booski.Enums;

namespace Booski.I18n;

public interface II18n {
    static Dictionary<Phrase, string>? Strings { get; set; }
}