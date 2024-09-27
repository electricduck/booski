using Booski.Enums;

namespace Booski.I18n;

public class Pt : II18n
{
    public static Dictionary<Phrase, string> Strings = new Dictionary<Phrase, string>() {
        { Phrase.Console_HelloWorld, "Olá, [0]!" },
        { Phrase.Console_PostHelpers_FirstRun, "Primeira execução. Armazenar em cache e ignorar todas as postagens" },
        { Phrase.Console_PostHelpers_PostAdded, "Adicionado: [0]" },
        { Phrase.Console_PostHelpers_PostCrossposted, "Postado em [0]: [1]" },
        { Phrase.Console_PostHelpers_PostCrosspostedError, "Não foi possível postar na [0]: [1]" },
        { Phrase.Console_PostHelpers_PostDeleted, "Eliminado: [0]" },
        { Phrase.Console_StartCommand_ClientConnected, "Conectado à [0]: [1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "Não é possível conectar-se à [0]" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "Daemon já em execução" },
        { Phrase.Console_StartCommand_DaemonError, "Não é possível executar como daemon" },
        { Phrase.Console_StartCommand_DaemonStarted, "Iniciou o daemon ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "Busca de postagens a cada [0] [1]. Altere isso com --sleep-time/-s (em segundos)" },
        { Phrase.Console_StartCommand_FetchingPostsError, "Não é possível armazenar postagens em cache" },
        { Phrase.SecondUnit_Single, "segundo" },
        { Phrase.SecondUnit_Multiple, "segundos" },
        { Phrase.SeeMore_Attachment, "🔗 Veja o anexo: [0]" },
        { Phrase.SeeMore_Photos, "📷 Veja as fotos: [0]" },
        { Phrase.SeeMore_Read, "➡️ Leia mais: [0]" },
        { Phrase.SeeMore_Video, "▶️ Assista ao vídeo: [0]" },
        { Phrase.SeeMoreRich_Attachment, "🔗 Ver anexo no Bluesky" },
        { Phrase.SeeMoreRich_Photos, "📷 Ver fotos no Bluesky" },
        { Phrase.SeeMoreRich_Video, "▶️ Assistir ao vídeo no Bluesky" },
        { Phrase.Sensitivity_Nudity, "Nudez" },
        { Phrase.Sensitivity_Porn, "Pornô" },
        { Phrase.Sensitivity_Suggestive, "Sugestiva" }
    };
}