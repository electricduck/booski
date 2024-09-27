using Booski.Enums;

namespace Booski.I18n;

public class Es : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "¡Hola, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Ver Anexo: [0]" },
        { Phrase.SeeMore_Photos, "📷 Ver Fotos: [0]" },
        { Phrase.SeeMore_Read, "➡️ Más información: [0]" },
        { Phrase.SeeMore_Video, "▶️ Ver Vídeo: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Ver archivo adjunto en Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Ver fotos en Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Ver Vídeo en Bluesky" },
        { Phrase.Sensitivity_Nudity, "Desnudez" },
        { Phrase.Sensitivity_Porn, "Porno" },
        { Phrase.Sensitivity_Suggestive, "Sugerente" }
    };
}