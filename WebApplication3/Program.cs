using ZaryaSite.Extentions;

namespace ZaryaSite;

public class Program
{
    internal static Dictionary<string, UserInfo> Saver = new()
    {
        {"IlyaKrolenko", new UserInfo { Login = "IlyaKrolenko", Password = "111", Role = Roles.Admin }},
        {"DimaRakov", new UserInfo { Login = "DimaRakov", Password = "333", Role = Roles.Admin }},
        {"IlyaKosov", new UserInfo { Login = "IlyaKosov", Password = "555", Role = Roles.Admin }}
    };

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAdminAuthentication();

        var app = builder.Build();
        app.UseStaticFiles();
        app.UseAuthentication();

        app.ConfigureUserEndpoints();

        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();

        app.ConfigureSiteEndpoints();

        app.Run();
    }
}
