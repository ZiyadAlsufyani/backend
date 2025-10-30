using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class RacingDbContext : DbContext
    {
        public RacingDbContext(DbContextOptions<RacingDbContext> options) : base(options)
        {
        }

        // DbSet properties for all models
        public DbSet<Stable> Stables { get; set; }
        public DbSet<Horse> Horses { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Owns> Owns { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure composite key for Owns table
            modelBuilder.Entity<Owns>()
                .HasKey(o => new { o.OwnerId, o.HorseId });

            // Configure relationships for Owns
            modelBuilder.Entity<Owns>()
                .HasOne(o => o.Owner)
                .WithMany(owner => owner.Owns)
                .HasForeignKey(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Owns>()
                .HasOne(o => o.Horse)
                .WithMany(horse => horse.Owns)
                .HasForeignKey(o => o.HorseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure composite key for RaceResults table
            modelBuilder.Entity<RaceResults>()
                .HasKey(rr => new { rr.RaceId, rr.HorseId });

            // Configure relationships for RaceResults
            modelBuilder.Entity<RaceResults>()
                .HasOne(rr => rr.Race)
                .WithMany(race => race.RaceResults)
                .HasForeignKey(rr => rr.RaceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RaceResults>()
                .HasOne(rr => rr.Horse)
                .WithMany(horse => horse.RaceResults)
                .HasForeignKey(rr => rr.HorseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Horse -> Stable relationship
            modelBuilder.Entity<Horse>()
                .HasOne(h => h.Stable)
                .WithMany(s => s.Horses)
                .HasForeignKey(h => h.StableId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Trainer -> Stable relationship
            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.Stable)
                .WithMany(s => s.Trainers)
                .HasForeignKey(t => t.StableId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Race -> Track relationship
            modelBuilder.Entity<Race>()
                .HasOne(r => r.Track)
                .WithMany(t => t.Races)
                .HasForeignKey(r => r.TrackName)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
