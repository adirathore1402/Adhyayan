using System.Text;
using Adhyayan.Core.Interfaces;
using Adhyayan.Infrastructure.Data;
using Adhyayan.Infrastructure.Repositories;
using Adhyayan.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

// ---------- Database ----------
builder.Services.AddDbContext<AdhyayanDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=adhyayan.db"));

// ---------- Repositories ----------
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IPracticeRepository, PracticeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ---------- Services ----------
builder.Services.AddScoped<ICurriculumLoaderService, CurriculumLoaderService>();
builder.Services.AddHttpClient<IQuestionGeneratorService, OpenAiQuestionGeneratorService>();

// ---------- JWT Auth ----------
var jwtKey = builder.Configuration["Jwt:Key"] ?? "AdhyayanSuperSecretKey2026!@#$%^&*()";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Adhyayan",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AdhyayanApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ---------- Controllers ----------
builder.Services.AddControllers();

var app = builder.Build();

// ---------- Auto-migrate & seed ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdhyayanDbContext>();
    await db.Database.EnsureCreatedAsync();

    var loader = scope.ServiceProvider.GetRequiredService<ICurriculumLoaderService>();
    await loader.SeedCurriculumFromConfigAsync();
}

// ---------- Middleware ----------
app.UseCors("AllowReactDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
