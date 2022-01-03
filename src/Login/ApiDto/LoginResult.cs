using Ivao.It.DiscordBot.Data.Entities;

namespace Ivao.It.DiscordLogin.ApiDto;

public class LoginResult
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public int result { get; set; }
    public string vid { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public int rating { get; set; }
    public int ratingatc { get; set; }
    public int ratingpilot { get; set; }
    public string division { get; set; }
    public string country { get; set; }
    public string skype { get; set; }
    public int hours_atc { get; set; }
    public int hours_pilot { get; set; }
    public int isNpoMember { get; set; }
    public int va_staff { get; set; }
    public string va_staff_ids { get; set; }
    public string va_staff_icaos { get; set; }
    public string staff { get; set; }
    public string va_member_ids { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    public static LoginResult Failed => new() { result = -1, vid = "FAILED" };

    #region Equality comparison override
    public override int GetHashCode() => int.Parse(this.vid);
    public override bool Equals(object? obj) => obj is LoginResult res && res.vid == this.vid;

    public static bool operator ==(LoginResult r1, LoginResult r2)
    {
        return (r1?.vid == r2?.vid);
    }
    public static bool operator !=(LoginResult r1, LoginResult r2)
    {
        return (r1.vid != r2.vid);
    }
    #endregion

    public static explicit operator User(LoginResult loginResult) =>
        new()
        {
            Vid = loginResult.vid,
            FirstName = loginResult.firstname,
            LastName = loginResult.lastname,
            GdprAcceptDate = DateTime.UtcNow,
        };
}
