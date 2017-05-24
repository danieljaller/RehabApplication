using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Rehab.MVC.DAL;

namespace Rehab.MVC.Migrations
{
    [DbContext(typeof(RehabContext))]
    [Migration("20170522111941_DropAndRecreateDb")]
    partial class DropAndRecreateDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Rehab.MVC.Models.Exercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int?>("ToolId");

                    b.Property<string>("VideoUrl");

                    b.HasKey("Id");

                    b.HasIndex("ToolId");

                    b.ToTable("Exercises");
                });

            modelBuilder.Entity("Rehab.MVC.Models.Tool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Tools");
                });

            modelBuilder.Entity("Rehab.MVC.Models.Workout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Workouts");
                });

            modelBuilder.Entity("Rehab.MVC.Models.WorkoutExercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ExerciseId");

                    b.Property<string>("Notes");

                    b.Property<int>("Reps");

                    b.Property<string>("Resistance");

                    b.Property<int>("Sets");

                    b.Property<int>("WorkoutId");

                    b.HasKey("Id");

                    b.HasIndex("ExerciseId");

                    b.HasIndex("WorkoutId");

                    b.ToTable("WorkoutExercises");
                });

            modelBuilder.Entity("Rehab.MVC.Models.WorkoutPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("WorkoutPlans");
                });

            modelBuilder.Entity("Rehab.MVC.Models.WorkoutPlanWorkout", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDone");

                    b.Property<DateTime>("ScheduledTime");

                    b.Property<int>("WorkoutId");

                    b.Property<int>("WorkoutPlanId");

                    b.HasKey("Id");

                    b.HasIndex("WorkoutPlanId");

                    b.HasIndex("WorkoutId", "WorkoutPlanId", "ScheduledTime")
                        .IsUnique();

                    b.ToTable("WorkoutPlanWorkouts");
                });

            modelBuilder.Entity("Rehab.MVC.Models.Exercise", b =>
                {
                    b.HasOne("Rehab.MVC.Models.Tool", "Tool")
                        .WithMany()
                        .HasForeignKey("ToolId");
                });

            modelBuilder.Entity("Rehab.MVC.Models.WorkoutExercise", b =>
                {
                    b.HasOne("Rehab.MVC.Models.Exercise", "Exercise")
                        .WithMany("WorkoutExercises")
                        .HasForeignKey("ExerciseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Rehab.MVC.Models.Workout", "Workout")
                        .WithMany("WorkoutExercises")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Rehab.MVC.Models.WorkoutPlanWorkout", b =>
                {
                    b.HasOne("Rehab.MVC.Models.Workout", "Workout")
                        .WithMany("WorkoutPlanWorkouts")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Rehab.MVC.Models.WorkoutPlan", "WorkoutPlan")
                        .WithMany("WorkoutPlanWorkouts")
                        .HasForeignKey("WorkoutPlanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
