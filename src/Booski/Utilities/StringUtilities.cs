namespace Booski.Utilities;

public class StringUtilities
{
    public static string Truncate(string value, int maxChars)
    {
        string ellipsis = "...";
        return value.Length <= maxChars ? value : value.Substring(0, maxChars - ellipsis.Length) + ellipsis;
    }
}