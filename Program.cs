using Microsoft.EntityFrameworkCore;
using rosa_testovoye.Data;

namespace rosa_testovoye;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddRazorPages();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DbInitializer.InitializeAsync(context);
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        await app.RunAsync();
    }
}
