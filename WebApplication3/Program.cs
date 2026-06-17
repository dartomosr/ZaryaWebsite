using ZaryaSite.Extentions;

namespace ZaryaSite;

public class Program
{
    public static Dictionary<string, string> Saver = new()
    {
        { "IlyaKosov", "555" },
        { "IlyaKrolenko", "111" },
        { "DimaRakov", "333" }
    };

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAdminAuthentication();

        var app = builder.Build();
        app.UseStaticFiles();
        app.UseAuthentication();

        app.ConfigureAdminEndpoints();

        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();

        app.ConfigureSiteEndpoints();

        app.Run();
    }
}
