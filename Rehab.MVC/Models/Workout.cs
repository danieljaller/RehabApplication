using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rehab.MVC.Models
{
    public class Workout
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}