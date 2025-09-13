using FluentValidation;
using FluentValidation.AspNetCore;
using IMDBClone.API.Services;
using IMDBClone.API.Settings;
using IMDBClone.API.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. MongoDbSettings konfigürasyonunu bağlama
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// 2. MongoClient’i DI Container’a ekleme (Singleton olarak)
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();

    return new MongoClient(settings.ConnectionString);
});

// 3. JWT Settings + UserService
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<UserService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

// 4. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<MovieCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// 5. MovieService servisimizin DI Container’a eklenmesi
builder.Services.AddSingleton<MovieService>();

// 6. Controller desteği
builder.Services.AddControllers();

// 7. OpenAPI / Scalar
builder.Services.AddOpenApi();

var app = builder.Build();

// 8. Development ortamında Swagger + Scalar aç
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// 9. Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 10. Uygulamayı çalıştır
app.Run();
