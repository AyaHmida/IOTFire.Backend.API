using IoTFire.Backend.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTFire.Backend.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email)
                      .IsUnique()
                      .HasDatabaseName("IX_users_email");

                entity.Property(u => u.Role)
                      .HasConversion<string>()
                      .HasMaxLength(20);

                entity.HasOne(u => u.ParentUser)
                      .WithMany()
                      .HasForeignKey(u => u.ParentUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);

                entity.Property(u => u.IsActive)
                      .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.Property(u => u.UpdatedAt)
                      .HasDefaultValueSql("NOW()");
            });
        }
    }
}
