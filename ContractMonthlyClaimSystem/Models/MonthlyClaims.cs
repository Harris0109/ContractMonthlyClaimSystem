using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaimSystem.Models
{
    public class MonthlyClaims
    {
        [Key]
        public int ClaimId { get; set; }

        [Display(Name = "Month")]
        [Range(1, 12, ErrorMessage = "Please select a valid month")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        [Range(2020, 2030, ErrorMessage = "Please enter a valid year")]
        public int Year { get; set; }

        [Display(Name = "Total Hours")]
        [Range(1, 744, ErrorMessage = "Hours must be between 1 and 744")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Hourly Rate")]
        [Range(50, 1000, ErrorMessage = "Hourly rate must be between R50 and R1000")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        // FIXED: Use string for Identity User ID
        public string LecturerId { get; set; } = string.Empty;

        // FIXED: Remove virtual Lecturer navigation property to avoid conflicts
        // public virtual Lecturer? Lecturer { get; set; }

        public virtual ICollection<SupportingDocument>? Documents { get; set; }

        [NotMapped]
        public IFormFile? UploadedFile { get; set; }

        [NotMapped]
        public string? ValidationMessage { get; set; }

        [NotMapped]
        public bool PassesAutoValidation { get; set; }

        // Auto-calculation method
        public void CalculateTotal()
        {
            TotalAmount = TotalHours * HourlyRate;
        }

        // Helper method for validation
        public void RunValidation()
        {
            var errors = new List<string>();

            if (TotalHours > 200)
                errors.Add("Hours exceed maximum limit (200 hours)");
            if (TotalHours < 1)
                errors.Add("Hours must be at least 1 hour");
            if (HourlyRate > 500)
                errors.Add("Hourly rate exceeds maximum limit (R500)");
            if (HourlyRate < 50)
                errors.Add("Hourly rate below minimum (R50)");
            if (TotalAmount > 100000)
                errors.Add("Total amount exceeds maximum limit (R100,000)");

            PassesAutoValidation = !errors.Any();
            ValidationMessage = PassesAutoValidation ?
                "Claim passes all automated checks" :
                $"Auto-validation failed: {string.Join("; ", errors)}";
        }
    }
}