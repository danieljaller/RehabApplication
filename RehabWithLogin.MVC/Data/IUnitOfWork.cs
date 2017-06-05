using System;
using RehabWithLogin.MVC.Data.Repository;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Exercise> ExerciseRepository { get; }
        IGenericRepository<WorkoutExercise> WorkoutExerciseRepository { get; }
        IGenericRepository<WorkoutPlan> WorkoutPlanRepository { get; }
        IGenericRepository<WorkoutPlanWorkout> WorkoutPlanWorkoutRepository { get; }
        IGenericRepository<Workout> WorkoutRepository { get; }
        IGenericRepository<Tool> ToolRepository { get; }

        void Save();
    }
}