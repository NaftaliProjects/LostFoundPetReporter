
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
using LostFoundPetReporter.CoreDb;
using LostFoundPetReporter.CoreDb.Models;

var factory = new PetReporterContextFactory();
using var context = factory.CreateDbContext(args);

// 2. Ensure database exists
context.Database.EnsureCreated();

Console.WriteLine("--- Testing EF Context ---");

// 3. Simple Create & Read Test
var testUser = new User
{
    Name = "Test User",
    Email = "test@example.com",
    Phone = "1234567890",
    HashedPassword = "hashed_secret"
};

context.Users.Add(testUser);
context.SaveChanges();

var savedUser = context.Users.FirstOrDefault(u => u.Email == "test@example.com");
Console.WriteLine($"User Created & Retrieved: ID {savedUser?.Id} - {savedUser?.Name}");



Console.WriteLine("--- Test Passed ---");

