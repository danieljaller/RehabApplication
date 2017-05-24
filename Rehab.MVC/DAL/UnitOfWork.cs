using System;
using Microsoft.EntityFrameworkCore;
using Rehab.MVC.DAL.Repository;
using Rehab.MVC.Models;

namespace Rehab.MVC.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        
        public UnitOfWork(RehabContext context)
        {
            _context = context;
            WorkoutPlanRepository = new GenericRepository<WorkoutPlan>(context);
            WorkoutExerciseRepository = new GenericRepository<WorkoutExercise>(context);
            ToolRepository = new GenericRepository<Tool>(context);
            WorkoutPlanWorkoutRepository = new GenericRepository<WorkoutPlanWorkout>(context);
            ExerciseRepository = new GenericRepository<Exercise>(context);
            WorkoutRepository = new GenericRepository<Workout>(context);
        }

        public IGenericRepository<Workout> WorkoutRepository { get; }
        public IGenericRepository<Exercise> ExerciseRepository { get; }
        public IGenericRepository<WorkoutPlanWorkout> WorkoutPlanWorkoutRepository { get; }
        public IGenericRepository<WorkoutExercise> WorkoutExerciseRepository { get; }
        public IGenericRepository<Tool> ToolRepository { get; }
        public IGenericRepository<WorkoutPlan> WorkoutPlanRepository { get; }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}