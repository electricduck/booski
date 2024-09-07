namespace Booski;

public class Say
{
    public static void Error(Exception e)
    {
        ConsoleMessage($"Error: {e.Message}", "🛑");
    }

    public static void Info(string message)
    {
        ConsoleMessage(message, "ℹ️");
    }

    public static void Success(string message)
    {
        ConsoleMessage(message, "✅");
    }

    public static void Warning(string message, string reason = "")
    {
        ConsoleMessage(message, "⚠️", reason);
    }

    static void ConsoleMessage(
        string message,
        string emoji = "",
        string reason = ""
    )
    {
        if(!String.IsNullOrEmpty(emoji))
            message = $"{emoji}  {message}";

        if(!String.IsNullOrEmpty(reason))
        {
            string padding = "";

            if(!String.IsNullOrEmpty(emoji))
                padding = new String(' ', emoji.Length + 1);

            message = $"{message}{Environment.NewLine}{padding}{reason}";
        }

        Console.WriteLine(message);
    }
}