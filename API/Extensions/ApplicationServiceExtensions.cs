using System;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        
          // Use the environment variable to fetch the connection string
          
        var connectionString = config.GetConnectionString("DefaultConnection");

        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseNpgsql(connectionString); // Ensure you're using Npgsql for PostgreSQL
        });


        services.AddCors();
        services.AddScoped<ItokenService, TokenService>();

        return services;
    }
}
