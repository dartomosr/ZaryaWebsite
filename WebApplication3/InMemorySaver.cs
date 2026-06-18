using System.Collections.Concurrent;

namespace ZaryaSite;

internal sealed class InMemorySaver : ISaver
{
    public IDictionary<string, UserInfo> Saver { get; } = new ConcurrentDictionary<string, UserInfo>(
        new Dictionary<string, UserInfo>
        {
            ["IlyaKrolenko"] = new UserInfo { Login = "IlyaKrolenko", Password = "111", Role = Roles.Admin },
            ["DimaRakov"] = new UserInfo { Login = "DimaRakov", Password = "333", Role = Roles.Admin },
            ["IlyaKosov"] = new UserInfo { Login = "IlyaKosov", Password = "555", Role = Roles.Admin }
        });
}
