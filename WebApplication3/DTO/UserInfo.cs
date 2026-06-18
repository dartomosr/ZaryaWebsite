namespace ZaryaSite;

internal class UserInfo
{
    internal required string Login { get; set; }

    internal required string Password { get; set; }

    internal string? Email { get; set; }

    internal required Roles Role { get; set; }
}
