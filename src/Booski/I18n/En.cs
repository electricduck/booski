using Booski.Enums;

namespace Booski.I18n;

public class En : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.SeeMore_Attachment, "🔗 See Attachment: [0]" },
        { Phrase.SeeMore_Photos, "📷 See Photos: [0]" },
        { Phrase.SeeMore_Read, "➡️ Read More: [0]" },
        { Phrase.SeeMore_Video, "▶️ Watch Video: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 See Attachment on Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 See Photos on Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Watch Video on Bluesky" },
        { Phrase.Sensitivity_Nudity, "Nudity" },
        { Phrase.Sensitivity_Porn, "Porn" },
        { Phrase.Sensitivity_Suggestive, "Suggestive" }
    };
}