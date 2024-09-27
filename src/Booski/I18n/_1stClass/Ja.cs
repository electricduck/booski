using Booski.Enums;

namespace Booski.I18n;

public class Ja : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "こんにちは、[0]！"},
        { Phrase.Console_PostHelpers_FirstRun, "最初の実行 キャッシュと全投稿の無視" },
        { Phrase.Console_PostHelpers_PostAdded, "追加：[0]" },
        { Phrase.Console_PostHelpers_PostCrossposted, "[0]に投稿されました：[1]" },
        { Phrase.Console_PostHelpers_PostCrosspostedError, "[0]に投稿できません：[1]" },
        { Phrase.Console_PostHelpers_PostDeleted, "削除された：[0]" },
        { Phrase.Console_PostHelpers_Ignoring, "無視：[0]"},
        { Phrase.Console_PostHelpers_IgnoringEmbedsButNotSupported, "投稿には埋め込みがあるが、どれもサポートされていない" },
        { Phrase.Console_PostHelpers_IgnoringOldCreatedAtDate, "投稿が[0][1]以上古い" },
        { Phrase.Console_PostHelpers_IgnoringReplyButNoParent, "投稿は返信だが、親が存在しない（削除されたか、無視されたか、当方ではない）" },
        { Phrase.Console_PostHelpers_IgnoringStartsWithMention, "投稿が「@」で始まっている" },
        { Phrase.Console_PostHelpers_PostNotDeleting, "[0]から削除されません：[1]"},
        { Phrase.Console_PostHelpers_PostNotDeletingErrorMastodonInstanceDomainNotMatch, "現在のインスタンスドメイン([0])がログに記録されたドメイン([1])と一致しない" },
        { Phrase.Console_StartCommand_ClientConnected, "[0]に接続：[1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "[0]に接続できない" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "すでに実行中のデーモン"},
        { Phrase.Console_StartCommand_DaemonError, "デーモンとして実行できない"},
        { Phrase.Console_StartCommand_DaemonStarted, "デーモン起動 ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "[0][1]ごとに記事を取得する。これを --sleep-time/-s (秒) で変更する" },
        { Phrase.Console_StartCommand_FetchingPostsError, "投稿をキャッシュできない" },
        { Phrase.Unit_Hour_Multiple, "時間" },
        { Phrase.Unit_Hour_Single, "時間" },
        { Phrase.Unit_Second_Multiple, "秒" },
        { Phrase.Unit_Second_Single, "秒" },
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