using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace API.Data;

public class DataContext(DbContextOptions options) :
IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>(options)
{
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<RecoveryRecord> RecoveryRecords { get; set; }
    public DbSet<ProfilePicture> ProfilePictures { get; set; }
    public DbSet<TrainerSubscription> TrainerSubscriptions { get; set; }
    public DbSet<TherapistSessionPrice> TherapistSessionPrices { get; set; }
    public DbSet<NutritionGuide> NutritionGuides { get; set; }
    public DbSet<MealAnalysis> MealAnalyses { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }


    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutPlanExercise> WorkoutPlanExercises { get; set; }
    
    public DbSet<TherapistExercise> TherapistExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<WorkoutPlanExercise>()
        .HasOne(wpe => wpe.WorkoutPlan)
        .WithMany(wp => wp.WorkoutPlanExercises)
        .HasForeignKey(wpe => wpe.WorkoutPlanId);

        builder.Entity<WorkoutPlanExercise>()
            .HasOne(wpe => wpe.Exercise)
            .WithMany(e => e.WorkoutPlanExercises)
            .HasForeignKey(wpe => wpe.ExerciseId);
        builder.Entity<AppUser>()
       .HasOne(u => u.ProfilePicture)
       .WithOne(p => p.User)
       .HasForeignKey<ProfilePicture>(p => p.UserId)
       .OnDelete(DeleteBehavior.Cascade);

    }
}
