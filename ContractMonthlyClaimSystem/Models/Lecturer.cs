using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Hourly Rate")]
        [Range(50, 1000, ErrorMessage = "Hourly rate must be between R50 and R1000")]
        public decimal HourlyRate { get; set; }

        // Link to Identity User
        public string? UserId { get; set; }

        // FIXED: Remove virtual collection to avoid circular references
        // public virtual ICollection<MonthlyClaims>? Claims { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}