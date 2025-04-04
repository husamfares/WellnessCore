using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<AppUser> Users { get; set; }  // Your users table
}
