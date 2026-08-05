global using LostFoundPetReporter.CoreDb;
global using Microsoft.EntityFrameworkCore;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


//add DI DB Context
var connetionString = builder.Configuration.GetConnectionString("TestENV");
builder.Services.AddDbContextPool<PetReporterContext>(
        options => options.UseSqlServer(connetionString,
            sqlOptions => sqlOptions.EnableRetryOnFailure().CommandTimeout(60))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


    

app.Run();
