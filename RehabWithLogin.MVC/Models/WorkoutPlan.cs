using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RehabWithLogin.MVC.Models
{
    public class WorkoutPlan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
    }
}