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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FamilyInvitation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Inviter)
                    .WithMany()
                    .HasForeignKey(e => e.InviterId)
                    .OnDelete(DeleteBehavior.Restrict); // lub inne zachowanie w przypadku usunięcia
            });
        }
    }
}
