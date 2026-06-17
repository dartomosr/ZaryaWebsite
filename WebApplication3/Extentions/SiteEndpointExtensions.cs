using System.Security.Claims;

namespace ZaryaSite.Extentions;

public static class SiteEndpointExtensions
{
    public static void ConfigureSiteEndpoints(this WebApplication app)
    {
        app.MapGet("/api/current-user", (ClaimsPrincipal user) => new
        {
            name = user.Identity?.Name,
            isAdmin = user.IsInRole("Admin")
        });

        app.MapGet("/main", async context =>
        {
            await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "html", "index.html"));
        });
        app.MapGet("/news.html", async context =>
        {
            await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "html", "news.html"));
        });
        app.MapGet("/news-1.html", async context =>
        {
            await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "html", "news-1.html"));
        });
        app.MapFallback(context =>
        {
            context.Response.Redirect("/main");
            return Task.CompletedTask;
        });
    }
}
