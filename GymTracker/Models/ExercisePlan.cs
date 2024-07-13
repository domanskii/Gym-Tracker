using System.ComponentModel.DataAnnotations;


namespace GymTracker.Models
{
    public class ExercisePlan
    {
        public int Id { get; set; }
        [Required]
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
        [Required]
        public int Repetitions { get; set; }
        [Required]
        public int Sets { get; set; }
        [Required]
        public double Weight { get; set; }
    }
}
