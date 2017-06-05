using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RehabWithLogin.MVC.Models
{
    public class WorkoutPlanWorkout
    {
        [Key]
        public int Id { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }
        public int WorkoutPlanId { get; set; }
        public Workout Workout { get; set; }
        public int WorkoutId { get; set; }
        public DateTime ScheduledTime { get; set; }

        [DefaultValue(false)]
        public bool IsDone { get; set; }
    }
}