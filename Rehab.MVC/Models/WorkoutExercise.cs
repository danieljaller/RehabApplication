using System.ComponentModel.DataAnnotations;

namespace Rehab.MVC.Models
{
    public class WorkoutExercise
    {
        [Key]
        public int Id { get; set; }

        public Workout Workout { get; set; }
        public int WorkoutId { get; set; }
        public Exercise Exercise { get; set; }
        public int ExerciseId { get; set; }
        public string Resistance { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
        public string Notes { get; set; }
    }
}