using System.Text;
using Catalog.Application;
using Catalog.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared;
using Users.Application;
using Users.Persistence;
using Users.Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Спільна інфраструктура — GlobalExceptionHandler, ProblemDetails
builder.Services.AddSharedInfrastructure();

// Catalog модуль
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogPersistence(builder.Configuration);

// Users модуль (схема identity)
builder.Services.AddUsersApplication(builder.Configuration);
builder.Services.AddUsersPersistence(builder.Configuration);

// Налаштування JWT Bearer Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Реєстрація контролерів з підтримкою презентаційних збірок модулів
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AuthController).Assembly);

// Swagger / OpenAPI з підтримкою тестування JWT авторизації
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KPS_Kurs API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Введіть: Bearer {ваш_токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Активує GlobalExceptionHandler для перетворення виключень у ProblemDetails
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
