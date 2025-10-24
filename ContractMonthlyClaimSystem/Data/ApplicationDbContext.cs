using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // FIX: Add explicit configuration to ignore System.Claims
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // This tells EF Core to ignore the System.Security.Claims.Claim class
            builder.Ignore<System.Security.Claims.Claim>();
        }

        public DbSet<MonthlyClaims> Claims { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
    }
}