using AppBskyLexicons = Booski.Lib.Lexicons.AppBsky;

namespace Booski.Common;

public class BskyState
{
    public string Did { get; set; }
    public string Handle { get; set; }
    public string PdsHost { get; set; }
    public AppBskyLexicons.Actor.Defs_ProfileViewDetailed Profile { get; set; }

    public void SetAdditionalFields()
    {
        if(
            Profile != null
        )
        {
            Did = Profile.Did;
            Handle = $"@{Profile.Handle}";
        }
    }
}