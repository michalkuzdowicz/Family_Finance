using Family_Finance.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Family_Finance.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FamilyInvitation> FamilyInvitations { get; set; }
        public DbSet<FamilyGroup> FamilyGroups { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<FinancialTarget> FinancialTarget { get; set; }
        public DbSet<TargetTransaction> TargetTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TargetTransaction>()
               .HasOne(tt => tt.FinancialTarget)
               .WithMany(ft => ft.TargetTransactions)
               .HasForeignKey(tt => tt.FinancialTargetId)
               .OnDelete(DeleteBehavior.Cascade);
        }

    }
}