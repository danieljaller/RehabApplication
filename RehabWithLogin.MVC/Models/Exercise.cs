using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RehabWithLogin.MVC.Models
{
    public class Exercise
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Tool Tool { get; set; }
        public string VideoUrl { get; set; }
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}