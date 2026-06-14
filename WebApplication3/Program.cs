using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Principal;

namespace ZaryaSite;

public class Program
{
    public static List<string> Roles = ["User", "Admin"];

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
        });

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/AdminAuthorization";
                options.AccessDeniedPath = "/main";
                options.Cookie.Name = "Admin.Authentication";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.SlidingExpiration = true;
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", police =>
            {
                police.RequireAuthenticatedUser();
                police.RequireRole("Admin");
            })
            .AddPolicy("CheckAuthorization", police =>
            {
                police.RequireAuthenticatedUser();
            });

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseAuthentication();

        app.MapGet("/AdminAuthorization", async context =>
        {
            var pass = context.Request.Query["pass"].ToString();
            
            if(pass != "111")
            {
                context.Response.Redirect("/main");
                return;
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            context.Response.Redirect("/main");
            return;
        });
        
        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();

        app.MapGet("/api/current-user", (ClaimsPrincipal user) => new
        {
            name = user.Identity?.Name,
            isAdmin = user.IsInRole("Admin")
        });

        app.MapGet("main", async context =>
        {
            await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "index.html"));
        });

        app.MapFallback(context =>
        {
            context.Response.Redirect("/main");
            return Task.CompletedTask;
        });

        app.Run();
    }
}
