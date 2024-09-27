using Booski.Enums;

namespace Booski.I18n;

public class Ja : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "こんにちは、[0]！"},
        { Phrase.Console_StartCommand_ClientConnected, "[0]に接続：[1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "[0]に接続できない" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "すでに実行中のデーモン"},
        { Phrase.Console_StartCommand_DaemonError, "デーモンとして実行できない"},
        { Phrase.Console_StartCommand_DaemonStarted, "デーモン起動 ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "[0][1]ごとに記事を取得する。これを --sleep-time/-s (秒) で変更する" },
        { Phrase.Console_StartCommand_FetchingPostsError, "投稿をキャッシュできない" },
        { Phrase.SecondUnit_Single, "秒" },
        { Phrase.SecondUnit_Multiple, "秒" },
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