using System.Text;

namespace Booski;

public class Say
{
    static bool IsLastSaySeparate { get; set; }

    public static void Custom(string message, string reason = "", string emoji = "", bool separate = false)
    {
        ConsoleMessage(message, emoji, reason, separate);
    }

    public static void Debug(string message, string reason = "")
    {
#if DEBUG
        ConsoleMessage(message, "⚙️", reason);
#endif
    }

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
        if(!IsLastSaySeparate)
        {
            Console.WriteLine(new String(separator, length));
            IsLastSaySeparate = true;
        }
    }

    public static void Success(string message)
    {
        ConsoleMessage(message, "✅");
    }

    public static void Warning(string message, string reason = "", bool separate = false)
    {
        ConsoleMessage(message, "⚠️", reason, separate);
    }

    static void ConsoleMessage(
        string message,
        string emoji = "",
        string reason = "",
        bool separate = false
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

        if(separate)
            Separate();
        
        Console.WriteLine(message);
        IsLastSaySeparate = false;

        if(separate)
            Separate();
    }
}