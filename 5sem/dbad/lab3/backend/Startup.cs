using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost3000",
            builder => builder
                .WithOrigins("http://localhost:3000")  // Разрешить запросы только с этого адреса
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

        // Другие сервисы
    }

    public void Configure(IApplicationBuilder app)
    {
        // Middleware и конфигурация
        app.UseCors("AllowLocalhost3000");

        // Другие middleware и настройки

        app.UseEndpoints(endpoints =>
        {
            // Настройка маршрутов
            endpoints.MapControllers();
        });
    }
}
