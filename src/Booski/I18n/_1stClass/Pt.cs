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
        { Phrase.Console_PostHelpers_Ignoring, "Ignorando: [0]"},
        { Phrase.Console_PostHelpers_IgnoringEmbedsButNotSupported, "A publicação tem incorporações, mas nenhuma é compatível" },
        { Phrase.Console_PostHelpers_IgnoringOldCreatedAtDate, "A postagem tem mais de [0] [1]" },
        { Phrase.Console_PostHelpers_IgnoringReplyButNoParent, "A postagem é uma resposta, mas o pai não existe (foi excluído, ignorado ou não é nosso)" },
        { Phrase.Console_PostHelpers_IgnoringStartsWithMention, "A postagem começa com \"@\"" },
        { Phrase.Console_PostHelpers_PostNotDeleting, "Não está sendo excluído do [0]: [1]" },
        { Phrase.Console_PostHelpers_PostNotDeletingErrorMastodonInstanceDomainNotMatch, "O domínio da instância atual ([0]) não corresponde ao domínio registrado ([1])" },
        { Phrase.Console_Program_FirstRun, "Olá, parece que você nunca executou o Booski antes!" },
        { Phrase.Console_Program_FirstRunEditConfig, "Edite a configuração em '[0]'" },
        { Phrase.Console_Program_UpdateAvailable, "Uma atualização está disponível!" },
        { Phrase.Console_Program_UpdateAvailableDownload, "Faça o download da versão [0] em [1]" },
        { Phrase.Console_StartCommand_ClientConnected, "Conectado à [0]: [1]"},
        { Phrase.Console_StartCommand_ClientConnectedError, "Não é possível conectar-se à [0]" },
        { Phrase.Console_StartCommand_DaemonAlreadyRunning, "Daemon já em execução" },
        { Phrase.Console_StartCommand_DaemonError, "Não é possível executar como daemon" },
        { Phrase.Console_StartCommand_DaemonStarted, "Iniciou o daemon ([0])" },
        { Phrase.Console_StartCommand_FetchingPosts, "Busca de postagens a cada [0] [1]. Altere isso com --sleep-time/-s (em segundos)" },
        { Phrase.Console_StartCommand_FetchingPostsError, "Não é possível armazenar postagens em cache" },
        { Phrase.Console_StopCommand_DaemonNotRunning, "Daemon não está em execução"},
        { Phrase.Console_StopCommand_DaemonError, "Não foi possível parar o daemon ([0])" },
        { Phrase.Console_StopCommand_DaemonStopped, "Daemon interrompido" },
        { Phrase.Unit_Hour_Multiple, "horas" },
        { Phrase.Unit_Hour_Single, "hora" },
        { Phrase.Unit_Second_Multiple, "segundos" },
        { Phrase.Unit_Second_Single, "segundo" },
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