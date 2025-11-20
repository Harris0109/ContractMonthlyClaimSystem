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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore<System.Security.Claims.Claim>();

            // REMOVE complex configuration to avoid relationship issues
            // Let EF Core handle relationships automatically
        }

        public DbSet<MonthlyClaims> Claims { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<SupportingDocument> SupportingDocuments { get; set; }
    }
}