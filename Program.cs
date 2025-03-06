//Program.cs
using System; // Para usar TimeSpan
using Microsoft.AspNetCore.Builder; // Para construir y configurar la aplicación web.
using Microsoft.Extensions.DependencyInjection; // Para configurar los servicios de la aplicación.
using Microsoft.Extensions.Hosting; // Para trabajar con diferentes entornos (desarrollo, producción, etc.).
using Microsoft.OpenApi.Models; // Para habilitar Swagger.
using csharpapi.Services; // Para tus servicios personalizados.
using csharapigenerica.Services; // Para TokenService y demás servicios.

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddSingleton<ControlConexion>();
builder.Services.AddSingleton<TokenService>(); // Se registra TokenService como singleton.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Habilitar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api Genérica C#",
        Version = "v1",
        Description = "API de prueba con ASP.NET Core y Swagger",
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Email = "soporte@miapi.com",
            Url = new Uri("https://miapi.com/contacto")
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Middleware de Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api Genérica C#");
        c.RoutePrefix = "swagger"; // Swagger estará en http://localhost:[puerto]/swagger
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");
app.UseSession();
app.UseAuthorization();

app.MapControllers();

// Endpoint adicional para probar TokenService
app.MapGet("/token", (TokenService tokenService) =>
{
    // Se genera un token para un usuario de ejemplo.
    var token = tokenService.GenerarToken("usuarioEjemplo");
    return Results.Ok(new { Token = token });
});

app.Run();
