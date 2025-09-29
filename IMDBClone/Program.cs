using FluentValidation;
using FluentValidation.AspNetCore;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using IMDBClone.API.Settings;
using IMDBClone.API.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------- CONFIGURATION --------------------
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);

// -------------------- DATABASE --------------------
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();

    return new MongoClient(settings.ConnectionString);
});

// -------------------- DATABASE COLLECTIONS --------------------
builder.Services.AddScoped<IMongoCollection<User>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<User>("Users");
});

builder.Services.AddScoped<IMongoCollection<Movie>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Movie>("Movies");
});

builder.Services.AddScoped<IMongoCollection<Actor>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Actor>("Actors");
});

builder.Services.AddScoped<IMongoCollection<Review>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = builder.Configuration
                          .GetSection("MongoDbSettings")
                          .Get<MongoDbSettings>();
    var database = client.GetDatabase(settings.DatabaseName);
    return database.GetCollection<Review>("Reviews");
});

// -------------------- SERVICES --------------------
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<ActorService>();
builder.Services.AddScoped<ReviewService>();

// -------------------- AUTHENTICATION & AUTHORIZATION --------------------
var jwtSettings = builder.Configuration
                         .GetSection("JwtSettings")
                         .Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

// -------------------- VALIDATION --------------------
builder.Services.AddValidatorsFromAssemblyContaining<MovieCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// -------------------- CONTROLLERS & OPENAPI --------------------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// -------------------- BUILD APP --------------------
var app = builder.Build();

// -------------------- MIDDLEWARE PIPELINE --------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
