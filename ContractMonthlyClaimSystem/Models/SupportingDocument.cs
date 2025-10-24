using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class SupportingDocument
    {
        [Key] // Added this line to explicitly define primary key
        public int DocumentId { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedDate { get; set; }

        // Foreign Key
        public int ClaimId { get; set; }

        // FIX: Change Claim to MonthlyClaims
        public virtual MonthlyClaims? Claim { get; set; }
    }
}
