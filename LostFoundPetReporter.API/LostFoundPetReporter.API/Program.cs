global using LostFoundPetReporter.API.ApiVersionSupport;
global using LostFoundPetReporter.API.Controllers.Base;
global using LostFoundPetReporter.CoreDb;
global using LostFoundPetReporter.CoreDb.Models;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.AspNetCore.Mvc.Versioning;
global using Microsoft.EntityFrameworkCore;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text.Json.Serialization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LostFoundPetReporter.API.Services.API;
using LostFoundPetReporter.API.Services.Authentication;
using LostFoundPetReporter.API.Services.BackgroundServices;
using LostFoundPetReporter.API.Services.Notification;
using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using LostFoundPetReporter.Services.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;





var builder = WebApplication.CreateBuilder(args);


FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(
        "Firebase/firebase-service-account.json")
});

// Add services to the container.
builder.Services.AddControllers(options =>
    {
        // Prevents ASP.NET from automatically requiring non-nullable navigation properties
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
   .AddJsonOptions(options =>
      {
          options.JsonSerializerOptions.PropertyNamingPolicy = null;
          options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
          options.JsonSerializerOptions.WriteIndented = true;
          options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Api versioning
builder.Services.AddLostFoundPetReporterApiVersionConfiguration(new ApiVersion(1, 0));




//add DI DB Context
var connetionString = builder.Configuration.GetConnectionString("TestENV");
builder.Services.AddDbContext<PetReporterContext>(options =>
    options.UseSqlServer(connetionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure().CommandTimeout(60))
);

builder.Services.AddScoped<IUserDeviceRepo, UserDeviceRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ILostReportRepo, LostReportRepo>();
builder.Services.AddScoped<IFoundReportRepo, FoundReportRepo>();
builder.Services.AddScoped<ILostFoundMatchRepo, LostFoundMatchRepo>();

builder.Services.AddSingleton<IMatchingQueue, MatchingQueue>();
builder.Services.AddHostedService<MatchingBackgroundService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();

builder.Services.AddSingleton<IExtFileQueue, ExtFileQueue>();
builder.Services.AddHostedService<ExtFileBackgroundService>();
builder.Services.AddScoped<IExtFileService, ExtFileService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IPushNotificationService, FirebasePushNotificationService>();


builder.Services.AddHttpClient<IAnimalDescriptionService, AnimalDescriptionService>();




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "LostFoundPetReporter API V1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();






app.Run();
