using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymTracker.Models
{
    public class Userr
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        
        public virtual IdentityUser? User { get; set; }

        [Required(ErrorMessage = "Pole Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Pole Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string? Email { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
