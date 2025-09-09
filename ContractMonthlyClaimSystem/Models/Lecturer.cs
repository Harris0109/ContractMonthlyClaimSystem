using System.Security.Claims;

namespace ContractMonthlyClaimSystem.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; } // Primary Key
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public decimal HourlyRate { get; set; }

        // Navigation Property: A Lecturer can have MANY Claims
        public virtual ICollection<Claim>? Claims { get; set; }
    }
}
