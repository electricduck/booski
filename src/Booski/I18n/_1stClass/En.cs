using Booski.Enums;

namespace Booski.I18n;

public class En : II18n {
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Hello, [0]!"},
        { Phrase.Console_PostHelpers_FirstRun, "First run. Caching and ignoring all previous posts" },
        { Phrase.Console_PostHelpers_PostAdded, "Added: [0]" },
        { Phrase.Console_PostHelpers_PostCrossposted, "Posted to [0]: [1]" },
        { Phrase.Console_PostHelpers_PostCrosspostedError, "Unable to post to [0]: [1]" },
        { Phrase.Console_PostHelpers_PostDeleted, "Deleted: [0]" },
        { Phrase.Console_PostHelpers_Ignoring, "Ignoring: [0]"},
        { Phrase.Console_PostHelpers_IgnoringEmbedsButNotSupported, "Post has embeds but none are supported" },
        { Phrase.Console_PostHelpers_IgnoringOldCreatedAtDate, "Post is older than [0] [1]" },
        { Phrase.Console_PostHelpers_IgnoringReplyButNoParent, "Post is a reply, but parent doesn't exist (either deleted, ignored or not ours)" },
        { Phrase.Console_PostHelpers_IgnoringStartsWithMention, "Post starts with \"@\"" },
        { Phrase.Console_PostHelpers_PostNotDeleting, "Not deleting from [0]: [1]" },
        { Phrase.Console_PostHelpers_PostNotDeletingErrorMastodonInstanceDomainNotMatch, "Current instance domain ([0]) does not match logged domain ([1])" },
        { Phrase.Console_Program_FirstRun, "Hey there, seems like you haven't ran Booski before!" },
        { Phrase.Console_Program_FirstRunEditConfig, "Edit the config at '[0]'" },
        { Phrase.Console_Program_UpdateAvailable, "An update is available!" },
        { Phrase.Console_Program_UpdateAvailableDownload, "Download [0] from [1]" },
        { Phrase.Console_StartCommand_ClientConnected, "Connecting to [0]: [1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "Unable to connect to [0]" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "Already running daemon"},
        { Phrase.Console_StartCommand_DaemonError, "Unable to run as daemon"},
        { Phrase.Console_StartCommand_DaemonStarted, "Started daemon ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "Fetching posts every [0] [1]. Change this with --sleep-time/-s (in seconds)" },
        { Phrase.Console_StartCommand_FetchingPostsError, "Unable to cache posts" },
        { Phrase.Unit_Second_Single, "second" },
        { Phrase.Unit_Second_Multiple, "seconds" },
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