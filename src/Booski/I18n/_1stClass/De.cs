using Booski.Enums;

namespace Booski.I18n;

public class De : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Hallo, [0]!"},
        { Phrase.Console_StartCommand_ClientConnected, "Verbinden mit [0]: [1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "Verbindung zu [0] nicht möglich" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "Bereits laufender Daemon"},
        { Phrase.Console_StartCommand_DaemonError, "Kann nicht als Daemon ausgeführt werden"},
        { Phrase.Console_StartCommand_DaemonStarted, "Gestarteter Daemon ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "Abruf von Beiträgen alle [0] [1]. Ändern Sie dies mit --sleep-time/-s (in Sekunden)" },
        { Phrase.Console_StartCommand_FetchingPostsError, "Beiträge können nicht zwischengespeichert werden" },
        { Phrase.SecondUnit_Single, "Sekunde" },
        { Phrase.SecondUnit_Multiple, "Sekunden" },
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