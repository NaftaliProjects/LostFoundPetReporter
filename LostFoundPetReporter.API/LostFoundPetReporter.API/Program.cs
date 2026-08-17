global using LostFoundPetReporter.CoreDb;
global using LostFoundPetReporter.CoreDb.Models;


global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Versioning;

global using System.Text.Json.Serialization;

global using LostFoundPetReporter.API.ApiVersionSupport;

global using LostFoundPetReporter.API.Controllers.Base;


using LostFoundPetReporter.CoreDb.Repos;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

using LostFoundPetReporter.Services.DataServices.Dal;
using LostFoundPetReporter.Services.DataServices.Interfaces;






var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<ILostReportRepo, LostReportRepo>();
builder.Services.AddScoped<IFoundReportRepo, FoundReportRepo>();

/*
builder.Services.AddScoped<IUserDataService, UserDalDataService>();
builder.Services.AddScoped<ILostReportDataService, LostReportDalDataService>();
builder.Services.AddScoped<IFoundReportDataService, FoundReportDalDataService>();
*/




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
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

app.UseAuthorization();

app.MapControllers();


    

app.Run();
