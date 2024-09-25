namespace Booski.Utilities;

public class EnvUtilities
{
    private readonly static string EnvPrefix = "Booski";

    private static string? GetEnv(string variable, bool prefix = true)
    {
        variable = prefix ? $"{EnvPrefix}_{variable}".ToUpper() : variable.ToUpper();
        string? output = Environment.GetEnvironmentVariable(variable);
        
        if(variable != "BOOSKI_DEBUG")
            Say.Debug($"{variable}: {(String.IsNullOrEmpty(output) ? "(Empty)" : output)}");
        
        return output;
    }

    public static bool GetEnvBool(string variable, bool prefix = true)
    {
        var output = GetEnv(variable, prefix);

        if(output == "true" || output == "1")
            return true;
        else
            return false;
    }

    public static string? GetEnvString(string variable, bool prefix = true)
    {
        return GetEnv(variable, prefix);
    }
}