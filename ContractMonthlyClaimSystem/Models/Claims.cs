using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Claims
    {
        public int ClaimId { get; set; } // Primary Key

        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string? Status { get; set; } // e.g., Submitted, Approved, Rejected
        public DateTime SubmittedDate { get; set; }

        // Foreign Key: This claim belongs to ONE Lecturer
        public int LecturerId { get; set; }
        // Navigation Property
        public virtual Lecturer? Lecturer { get; set; }

        // A Claim can have MANY SupportingDocuments
        public virtual ICollection<SupportingDocument>? Documents { get; set; }
    }
}
