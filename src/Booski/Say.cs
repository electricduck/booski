using System.Text;

namespace Booski;

public class Say
{
    public static void Error(Exception e)
    {
        ConsoleMessage($"Error: {e.Message}", "🛑");
    }

    public static void Info(string message, string reason = "")
    {
        ConsoleMessage(message, "ℹ️", reason);
    }

    public static void Separate(int length = 80, char separator = '-')
    {
        Console.WriteLine(new String(separator, length));
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
        var emojiByteLength = Encoding.UTF8.GetBytes(emoji).Length;
        var emojiPadding = 0;

        // i don't fuckin understand unicode
        switch(emojiByteLength)
        {
            case 3:
            case 4:
                emojiPadding = 1;
                break;
            case 5:
            case 6:
                emojiPadding = 2;
                break;
        }

        var emojiPaddingString = new String(' ', emojiPadding);

        if(!String.IsNullOrEmpty(emoji))
            message = $"{emoji}{emojiPaddingString}{message}";

        if(!String.IsNullOrEmpty(reason))
        {
            var reasonPadding = $"{emojiPaddingString} ";
            message = $"{message}{Environment.NewLine}{reasonPadding}{reason}";
        }

        Console.WriteLine(message);
    }
}