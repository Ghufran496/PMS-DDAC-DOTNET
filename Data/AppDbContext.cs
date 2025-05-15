using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PMS_DDAC.Models;

namespace PMS_DDAC.Data
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PropertyModel> Properties { get; set; } = null!;
        public DbSet<RoomModel> Rooms { get; set; } = null!;
        public DbSet<BillModel> Bills { get; set; } = null!;
        public DbSet<ServiceModel> Services { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure Identity tables are configured

            // 🔹 Disable cascade delete for Property-Bill relationship
            modelBuilder.Entity<BillModel>()
                .HasOne(b => b.Property)
                .WithMany()
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict); // 🚀 Fix: Prevent cascading deletes

            // 🔹 Disable cascade delete for Bill-Tenant relationship
            modelBuilder.Entity<BillModel>()
                .HasOne(b => b.Tenant)
                .WithMany()
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict); // 🚀 Prevent cascade path conflicts

            // 🔹 Property-Owner Relationship (Owner can have multiple properties)
            modelBuilder.Entity<PropertyModel>()
                .HasOne(p => p.Owner)
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
