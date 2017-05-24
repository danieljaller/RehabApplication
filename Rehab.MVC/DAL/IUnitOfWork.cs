using System;
using Rehab.MVC.DAL.Repository;
using Rehab.MVC.Models;

namespace Rehab.MVC.DAL
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