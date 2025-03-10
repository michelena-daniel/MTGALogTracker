using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {            
        }

        public DbSet<UserInfo> Users { get; set; }
        public DbSet<PlayerRank> PlayerRanks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerRank>()
            .HasOne(p => p.User)
            .WithMany(u => u.PlayerRanks)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserInfo>()
           .HasIndex(u => u.UserNameWithCode)
           .IsUnique();

            modelBuilder.Entity<PlayerRank>()
           .HasIndex(u => u.LogId)
           .IsUnique();

            modelBuilder.Entity<PlayerRank>()
            .Property(p => p.TimeStamp)
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
