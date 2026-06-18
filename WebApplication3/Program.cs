using ZaryaSite.Extentions;

namespace ZaryaSite;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAdminAuthentication();
        builder.Services.AddSingleton<ISaver, InMemorySaver>();

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
