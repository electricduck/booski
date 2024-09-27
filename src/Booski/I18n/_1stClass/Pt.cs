using Booski.Enums;

namespace Booski.I18n;

public class Pt : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Olá, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Veja o anexo: [0]" },
        { Phrase.SeeMore_Photos, "📷 Veja as fotos: [0]" },
        { Phrase.SeeMore_Read, "➡️ Leia mais: [0]" },
        { Phrase.SeeMore_Video, "▶️ Assista ao vídeo: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Ver anexo no Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Ver fotos no Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Assistir ao vídeo no Bluesky" },
        { Phrase.Sensitivity_Nudity, "Nudez" },
        { Phrase.Sensitivity_Porn, "Pornô" },
        { Phrase.Sensitivity_Suggestive, "Sugestiva" }
    };
}