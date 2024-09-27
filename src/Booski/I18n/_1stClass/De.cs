using Booski.Enums;

namespace Booski.I18n;

public class De : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Hallo, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Siehe Beilage: [0]" },
        { Phrase.SeeMore_Photos, "📷 Siehe Fotos: [0]" },
        { Phrase.SeeMore_Read, "➡️ Mehr lesen: [0]" },
        { Phrase.SeeMore_Video, "▶️ Video ansehen: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Siehe Anlage auf Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Fotos auf Bluesky ansehen" },
        { Phrase.SeeMoreRich_Video, "▶️ Video ansehen auf Bluesky" },
        { Phrase.Sensitivity_Nudity, "Nacktheit" },
        { Phrase.Sensitivity_Porn, "Porno" },
        { Phrase.Sensitivity_Suggestive, "Anzüglich" }
    };
}