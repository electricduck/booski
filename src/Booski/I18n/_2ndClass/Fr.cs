using Booski.Enums;

namespace Booski.I18n;

public class Fr : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Bonjour, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Voir la pièce jointe : [0]" },
        { Phrase.SeeMore_Photos, "📷 Voir les photos : [0]" },
        { Phrase.SeeMore_Read, "➡️ En savoir plus : [0]" },
        { Phrase.SeeMore_Video, "▶️ Voir la vidéo : [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Voir la pièce jointe sur Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Voir les photos sur Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Voir la vidéo sur Bluesky" },
        { Phrase.Sensitivity_Nudity, "Nudité" },
        { Phrase.Sensitivity_Porn, "Porno" },
        { Phrase.Sensitivity_Suggestive, "Suggestif" }
    };
}