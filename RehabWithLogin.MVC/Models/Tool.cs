using System.ComponentModel.DataAnnotations.Schema;

namespace RehabWithLogin.MVC.Models
{
    public class Tool
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserEmail { get; set; }
        public string Name { get; set; }
    }
}