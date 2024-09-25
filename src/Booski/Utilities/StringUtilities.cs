namespace Booski.Utilities;

public class StringUtilities
{
    public static bool ConvertToBool(string? boolString)
    {
        if(boolString == "true" || boolString == "1")
            return true;
        else
            return false;
    }

    public static string Truncate(string value, int maxChars)
    {
        string ellipsis = "...";
        return value.Length <= maxChars ? value : value.Substring(0, maxChars - ellipsis.Length) + ellipsis;
    }
}