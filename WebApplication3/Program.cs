using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Security.Principal;

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

        builder.Services.AddControllers();
        builder.Services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
        });

        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Admin/Login";
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

        app.MapGet("/Admin/Login", (IWebHostEnvironment env) =>
        {
            return Results.File(
                Path.Combine(env.WebRootPath, "LoginAndRegister.html"),
                "text/html; charset=utf-8");
        });
        
        app.MapPost("/Admin/Login/Send", async (HttpContext context) =>
        {
            var form = await context.Request.ReadFormAsync();
            var login = form["Login"].ToString();
            var pass = form["Password"].ToString();
            if (string.IsNullOrEmpty(login))
            {
                return Results.Redirect($"/Admin/Login?error=nullLogin&login={login}");
            }
            else if(string.IsNullOrEmpty(pass))
            {
                return Results.Redirect($"/Admin/Login?error=nullPass&login={login}");
            }
            else if(!Saver.ContainsKey(login))
            {
                return Results.Redirect($"/Admin/Login?error=wrongLogin&login={login}");
            }
            else if (!Saver.TryGetValue(login, out var savedPass) || savedPass != pass)
            {
                return Results.Redirect($"/Admin/Login?error=wrongPass&login={login}");
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
            return Results.Redirect("/main");
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
