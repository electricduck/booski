using Booski.Enums;

namespace Booski.I18n;

public class Ru : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Здравствуйте, [0]!"},
        { Phrase.SeeMore_Attachment, "🔗 Посмотреть вложение: [0]" },
        { Phrase.SeeMore_Photos, "📷 Посмотреть фотографии: [0]" },
        { Phrase.SeeMore_Read, "➡️ Читать дальше: [0]" },
        { Phrase.SeeMore_Video, "▶️ Смотреть видео: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Посмотреть вложение на Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Смотреть фотографии на Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Смотреть видео на Bluesky" },
        { Phrase.Sensitivity_Nudity, "Обнаженка" },
        { Phrase.Sensitivity_Porn, "Порно" },
        { Phrase.Sensitivity_Suggestive, "Суггестивная" }
    };
}