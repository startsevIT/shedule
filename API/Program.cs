using System.Text;
using API.Controllers;
using Application.Services.Users;
using Domain.Entity;
using Domain.Users;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection(nameof(SmtpSettings)));
builder.Services.Configure<AdminCredentials>(builder.Configuration.GetSection(nameof(AdminCredentials)));

builder.Services.AddSingleton<WebSocketHub>();
builder.Services.AddScoped<IDbContext, SqLiteDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddServices();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Определяем схему безопасности
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header с использованием Bearer. Пример: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Применяем схему ко всем эндпоинтам
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "Bearer"
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();

internal static class ServiceRegistrator
{
    public static void AddServices(this IServiceCollection services)
        => AssemblyReflectionExtensions.GetServices()
            .ForEach(x => services.AddScoped(x));
}