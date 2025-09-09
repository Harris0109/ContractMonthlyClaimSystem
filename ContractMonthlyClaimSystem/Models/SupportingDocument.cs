using System.Security.Claims;

namespace ContractMonthlyClaimSystem.Models
{
    public class SupportingDocument
    {
        public int DocumentId { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FilePath { get; set; } // Where the file is saved on the server
        public DateTime UploadedDate { get; set; }

        // Foreign Key: This document belongs to ONE Claim
        public int ClaimId { get; set; }
        
        // Navigation Property
        public virtual Claim? Claim { get; set; }
    }
}
