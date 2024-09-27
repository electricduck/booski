using Booski.Enums;

namespace Booski.I18n;

public class Fi : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.SeeMore_Attachment, "🔗 Katso liite: [0]" },
        { Phrase.SeeMore_Photos, "📷 Katso kuvat: [0]" },
        { Phrase.SeeMore_Read, "➡️ Lue lisää: [0]" },
        { Phrase.SeeMore_Video, "▶️ Katso video: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Katso liite Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Katso valokuvia Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Katso videota Bluesky" },
        { Phrase.Sensitivity_Nudity, "Alastomuus" },
        { Phrase.Sensitivity_Porn, "Porno" },
        { Phrase.Sensitivity_Suggestive, "Viitteellinen" }
    };
}