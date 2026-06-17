using Microsoft.AspNetCore.Authentication.Cookies;

namespace ZaryaSite.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminAuthentication(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRazorPages(options =>
        {
            options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
        });

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(ConfigureAdminCookieOptions);

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", police =>
            {
                police.RequireAuthenticatedUser();
                police.RequireRole("Admin");
            })
            .AddPolicy("CheckAuthorization", police =>
            {
                police.RequireAuthenticatedUser();
            });

        return services;
    }

    private static void ConfigureAdminCookieOptions(CookieAuthenticationOptions cookieOptions)
    {
        cookieOptions.LoginPath = "/Admin/Login";
        cookieOptions.AccessDeniedPath = "/main";
        cookieOptions.Cookie.Name = "Admin.Authentication";
        cookieOptions.Cookie.HttpOnly = true;
        cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        cookieOptions.Cookie.SameSite = SameSiteMode.Lax;
        cookieOptions.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        cookieOptions.SlidingExpiration = true;
    }
}
