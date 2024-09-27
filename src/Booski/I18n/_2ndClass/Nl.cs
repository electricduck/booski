using Booski.Enums;

namespace Booski.I18n;

public class Nl : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Hallo, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Zie bijlage: [0]" },
        { Phrase.SeeMore_Photos, "📷 Foto's bekijken: [0]" },
        { Phrase.SeeMore_Read, "➡️ Lees meer: [0]" },
        { Phrase.SeeMore_Video, "▶️ Bekijk video: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Zie Bijlage op Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Bekijk foto's op Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Bekijk Video op Bluesky" },
        { Phrase.Sensitivity_Nudity, "Naaktheid" },
        { Phrase.Sensitivity_Porn, "Porno" },
        { Phrase.Sensitivity_Suggestive, "Suggestief" }
    };
}