using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
                public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<Tool> Tools { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server = (localdb)\\mssqllocaldb; Database = Rehab; Trusted_Connection = True; ");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkoutPlanWorkout>().HasIndex(x => new { x.WorkoutId, x.WorkoutPlanId, x.ScheduledTime}).IsUnique(true);

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.WorkoutPlan)
                .WithMany(w => w.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutPlanId);

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.Workout)
                .WithMany(w => w.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutId);
        }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    // Customize the ASP.NET Identity model and override the defaults if needed.
        //    // For example, you can rename the ASP.NET Identity table names and more.
        //    // Add your customizations after calling base.OnModelCreating(builder);
        //}
    }
}
