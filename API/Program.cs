using System.Text;
using Application.Services.Users;
using Domain.Entity;
using Domain.Users;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var jwtConfig = builder.Configuration.GetSection("JwtSettings");
var smtpConfig = builder.Configuration.GetSection("SmtpSettings");
var adminCredentials = builder.Configuration.GetSection("AdminCredentials");
builder.Services.Configure<JwtSettings>(jwtConfig);
builder.Services.Configure<SmtpSettings>(smtpConfig);
builder.Services.Configure<AdminCredentials>(adminCredentials);

builder.Services.AddScoped<IDbContext, SqLiteDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = jwtConfig.Get<JwtSettings>();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience =  false,
            ValidateIssuer =  false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddRepositories();
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
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();

public static class ServiceRegistrator
{
    extension(IServiceCollection services)
    {
        public void AddServices()
            => AssemblyReflectionExtensions.GetServices()
                .ForEach(x => services.AddScoped(x));

        public void AddRepositories()
            => AssemblyReflectionExtensions.GetRepositories()
                .ForEach(x => services.AddScoped(x.Interface, x.Implementation));
    }
}