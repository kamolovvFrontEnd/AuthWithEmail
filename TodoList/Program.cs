using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TodoList.Core.Dto;
using TodoList.Core.Entities;
using TodoList.Infrastructure;
using TodoList.Infrastructure.Database;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var contextDb = builder.Services.AddDbContext<Data>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes("kdb_is_the_best_midfielder_in_history_of_football")), // Секретный ключ
            ValidateIssuer = false, // Не валидируем издателя
            ValidateAudience = false, // Не валидируем аудиторию
            RequireExpirationTime = true, // Требуем указания срока действия
            ValidateLifetime = true // Валидируем срок действия токена
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    // Определяем схему аутентификации через JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "Введите токен JWT следующим образом: Bearer {токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Настраиваем требования для безопасности
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
            new string[] { }
        }
    });
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ToDoService>();
builder.Services.AddScoped<SendEmailService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ResetPasswordService>();
builder.Services.AddScoped<RegisterRequest>();


// Добавляем кэш для сессий
builder.Services.AddDistributedMemoryCache(); // Хранение данных в памяти (для разработки)

// Добавляем поддержку сессий
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии (напр., 30 минут)
    options.Cookie.HttpOnly = true; // Cookie доступно только через HTTP
    options.Cookie.IsEssential = true; // Обязательная часть для работы сайта
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Далее в методе Configure нужно добавить middleware для сессий
app.UseSession();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();