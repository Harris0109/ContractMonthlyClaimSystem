using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services
{
    public class ClaimValidationService
    {
        public (bool isValid, string message) ValidateClaim(MonthlyClaims claim)
        {
            var errors = new List<string>();

            // Predefined validation criteria
            if (claim.TotalHours > 200)
                errors.Add("Hours exceed maximum limit (200 hours)");

            if (claim.TotalHours < 1)
                errors.Add("Hours must be at least 1 hour");

            if (claim.HourlyRate > 500)
                errors.Add("Hourly rate exceeds maximum limit (R500)");

            if (claim.HourlyRate < 50)
                errors.Add("Hourly rate below minimum (R50)");

            if (claim.TotalAmount > 100000)
                errors.Add("Total amount exceeds maximum limit (R100,000)");

            // Check for reasonable hours per day (assuming 22 working days)
            var averageHoursPerDay = claim.TotalHours / 22;
            if (averageHoursPerDay > 12)
                errors.Add($"Average {averageHoursPerDay:F1} hours per day seems high");

            if (errors.Any())
            {
                return (false, $"Auto-validation failed: {string.Join("; ", errors)}");
            }

            return (true, "Claim passes all automated checks");
        }
    }
}