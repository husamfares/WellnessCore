using API.Extensions;
<<<<<<< HEAD
using API.Helpers;
=======
using API.Middleware;
using Microsoft.AspNetCore.Builder;
>>>>>>> 56f0f84ac3016c75f5c71b2490ee6a937fb5df45


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.UseDeveloperExceptionPage();//for more info about exceptions

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
