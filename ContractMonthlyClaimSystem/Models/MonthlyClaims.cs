using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaimSystem.Models
{
    public class MonthlyClaims
    {
        [Key] // Added this line to explicitly define primary key
        public int ClaimId { get; set; }

        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Total Hours")]
        public decimal TotalHours { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        public string LecturerId { get; set; } = string.Empty;

        public virtual Lecturer? Lecturer { get; set; }

        public virtual ICollection<SupportingDocument>? Documents { get; set; }

        [NotMapped]
        public IFormFile? UploadedFile { get; set; }
    }
}