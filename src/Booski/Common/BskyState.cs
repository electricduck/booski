using AppBskyResponses = Booski.Lib.Internal.AppBsky.Responses;

namespace Booski.Common;

public class BskyState
{
    public string Did { get; set; }
    public string Handle { get; set; }
    public AppBskyResponses.GetProfileResponse Profile { get; set; }

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