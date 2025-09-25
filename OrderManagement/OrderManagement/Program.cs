using OrderManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors();
            builder.Services.AddControllers();

            builder.Services.AddHttpClient();
            builder.Services.AddMemoryCache();

            var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);                 
            var dbFile = Path.Combine(dataDir, "app.db");

            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite($"Data Source={dbFile}"));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
                    ctx.Context.Response.Headers["Pragma"] = "no-cache";
                    ctx.Context.Response.Headers["Expires"] = "0";
                    ctx.Context.Response.Headers["Surrogate-Control"] = "no-store";
                }
            });
            app.MapControllers();

            app.Run();
        }
    }
}
