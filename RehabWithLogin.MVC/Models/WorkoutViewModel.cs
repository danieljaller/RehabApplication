using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RehabWithLogin.MVC.Models
{
    public class WorkoutViewModel
    {
        public int Id { get; set; }

        public string UserEmail { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
        public IEnumerable<Tool> Tools { get; set; }
    }
}
