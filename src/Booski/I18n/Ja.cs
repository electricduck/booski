using Booski.Enums;

namespace Booski.I18n;

public class Ja : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.SeeMore_Attachment, "🔗 添付ファイルを見る: [0]" },
        { Phrase.SeeMore_Photos, "📷 写真を見る: [0]" },
        { Phrase.SeeMore_Read, "➡️ 続きを読む: [0]" },
        { Phrase.SeeMore_Video, "▶️ ビデオを見る: [0]"},
        { Phrase.SeeMoreRich_Attachment, "🔗 Blueskyでアタッチメントを見る" },
        { Phrase.SeeMoreRich_Photos, "📷 Blueskyで写真を見る" },
        { Phrase.SeeMoreRich_Video, "▶️ Blueskyでビデオを見る" },
        { Phrase.Sensitivity_Nudity, "ヌード" },
        { Phrase.Sensitivity_Porn, "ポルノ" },
        { Phrase.Sensitivity_Suggestive, "暗示的" }
    };
}