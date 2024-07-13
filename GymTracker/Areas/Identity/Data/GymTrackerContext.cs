using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymTracker.Models;

namespace GymTracker.Data;

public class GymTrackerContext : IdentityDbContext<IdentityUser>
{
    public GymTrackerContext(DbContextOptions<GymTrackerContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }


    public DbSet<ExercisePlan>? ExercisePlans { get; set; }

    public DbSet<TrainingPlan>? TrainingPlans { get; set; }

    public DbSet<Userr>? Userrs { get; set; }
    
    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<Offer> Offers { get; set; }

}
