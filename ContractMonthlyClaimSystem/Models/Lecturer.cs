using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Lecturer
    {
        [Key] // Added this line to explicitly define primary key
        public int LecturerId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public decimal HourlyRate { get; set; }
        public string? UserId { get; set; }


        public virtual ICollection<MonthlyClaims>? Claims { get; set; }
    }
}