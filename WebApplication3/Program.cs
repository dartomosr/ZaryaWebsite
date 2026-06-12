using System.Security.Claims;

namespace WebApplication3
{
    public class Program
    {

        public static List<string> Roles = ["User", "Admin"];

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                var role = context.Request.Query["user"].ToString();
                
                if(Roles.Contains(role))
                {
                    context.Response.Cookies.Append("User", role);
                }
                else
                {
                    role = context.Request.Cookies["User"] ?? "User";
                }

                
                var claims = new[]
                {
                    new Claim(ClaimTypes.Role, role)
                };

                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "FakeAdmin"));
                await next();
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
}
