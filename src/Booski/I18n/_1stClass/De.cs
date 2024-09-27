using Booski.Enums;

namespace Booski.I18n;

public class De : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Hallo, [0]!"},
        { Phrase.Console_PostHelpers_FirstRun, "Erster Lauf. Zwischenspeichern und Ignorieren aller Beiträge" },
        { Phrase.Console_PostHelpers_PostAdded, "Hinzugefügt: [0]" },
        { Phrase.Console_PostHelpers_PostCrossposted, "Eingestellt bei [0]: [1]" },
        { Phrase.Console_PostHelpers_PostCrosspostedError, "Kann nicht an [0] gepostet werden: [1]" },
        { Phrase.Console_PostHelpers_PostDeleted, "Gelöscht: [0]" },
        { Phrase.Console_PostHelpers_Ignoring, "Ignorieren: [0]"},
        { Phrase.Console_PostHelpers_IgnoringEmbedsButNotSupported, "Beitrag hat Einbettungen, aber keine werden unterstützt" },
        { Phrase.Console_PostHelpers_IgnoringOldCreatedAtDate, "Der Beitrag ist älter als [0] [1]" },
        { Phrase.Console_PostHelpers_IgnoringReplyButNoParent, "Der Beitrag ist eine Antwort, aber der Elternteil existiert nicht (entweder gelöscht, ignoriert oder nicht von uns)" },
        { Phrase.Console_PostHelpers_IgnoringStartsWithMention, "Beitrag beginnt mit „@“" },
        { Phrase.Console_PostHelpers_PostNotDeleting, "Keine Löschung von [0]: [1]" },
        { Phrase.Console_PostHelpers_PostNotDeletingErrorMastodonInstanceDomainNotMatch, "Aktuelle Instanzdomäne ([0]) stimmt nicht mit protokollierter Domäne ([1]) überein" },
        { Phrase.Console_Program_FirstRun, "Hallo, anscheinend hast du Booski noch nicht benutzt!" },
        { Phrase.Console_Program_FirstRunEditConfig, "Bearbeiten Sie die Konfiguration unter '[0]'" },
        { Phrase.Console_Program_UpdateAvailable, "Ein Update ist verfügbar!" },
        { Phrase.Console_Program_UpdateAvailableDownload, "Download [0] von [1]" },
        { Phrase.Console_StartCommand_ClientConnected, "Verbinden mit [0]: [1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "Verbindung zu [0] nicht möglich" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "Bereits laufender Daemon"},
        { Phrase.Console_StartCommand_DaemonError, "Kann nicht als Daemon ausgeführt werden"},
        { Phrase.Console_StartCommand_DaemonStarted, "Gestarteter Daemon ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "Abruf von Beiträgen alle [0] [1]. Ändern Sie dies mit --sleep-time/-s (in Sekunden)" },
        { Phrase.Console_StartCommand_FetchingPostsError, "Beiträge können nicht zwischengespeichert werden" },
        { Phrase.Unit_Hour_Multiple, "Stunden" },
        { Phrase.Unit_Hour_Single, "Stunde" },
        { Phrase.Unit_Second_Multiple, "Sekunden" },
        { Phrase.Unit_Second_Single, "Sekunde" },
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