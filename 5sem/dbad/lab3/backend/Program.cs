using backend;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure services in Startup.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=example;Pooling=true;SearchPath=lab3")).AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost3000",
            builder => builder
                .WithOrigins("http://127.0.0.1:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    }).AddControllers();

var app = builder.Build();
var env = app.Environment;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Enable CORS middleware
app.UseCors("AllowLocalhost3000");

// Маршрутизация контроллеров
app.MapControllers();

app.Run();
