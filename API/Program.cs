using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using API.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddHttpClient(); // ✅ This registers IHttpClientFactory


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddHttpClient();
builder.Services.AddScoped<ExerciseImportService>();
builder.Services.AddScoped<CloudinaryService>();




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.UseDeveloperExceptionPage();//for more info about exceptions

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
 
 using var scope = app.Services.CreateScope();
 var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

    await context.Database.MigrateAsync();
    await Seed.SeedRolesAsync(userManager, roleManager);

     //   Seed predefined workout plans
    var seeder = new WorkoutPlanSeeder(context);
    await seeder.SeedPredefinedPlansAsync();

        var httpClient = httpClientFactory.CreateClient();
    var exerciseImportService = new ExerciseImportService(httpClient, context);

if (await context.Exercises.CountAsync() < 480)
{
    Console.WriteLine("Seeding exercises from external API...");
    await exerciseImportService.ImportExercisesFromApiAsync(480);
}

}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
