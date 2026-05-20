using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using rosa_testovoye.Api;
using rosa_testovoye.Data;
using rosa_testovoye.Services;

namespace rosa_testovoye;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        var sqlitePath = Path.Combine(dataDirectory, "rosa.db");
        var dataProtectionKeys = Path.Combine(dataDirectory, "keys");
        Directory.CreateDirectory(dataProtectionKeys);

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeys))
            .SetApplicationName("rosa_testovoye");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={sqlitePath}"));

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".rosa.Session";
            options.IdleTimeout = TimeSpan.FromHours(8);
        });

        builder.Services.AddScoped<ICertificateRequestService, CertificateRequestService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Сервис справок ORP",
                Version = "v1",
                Description = "API заявок на бухгалтерские справки"
            });
        });
        builder.Services.AddRazorPages();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DbInitializer.InitializeAsync(context);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Сервис справок ORP v1");
            });
            app.UseHttpsRedirection();
        }
        app.UseRouting();
        app.UseSession();
        app.UseAuthorization();
        app.MapEmployeeApi();
        app.MapCertificateRequestApi();
        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();

        await app.RunAsync();
    }
}
