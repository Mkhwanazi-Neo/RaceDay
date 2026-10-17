using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : base(options)
        {
        }

        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // ORGANISER Entity Configuration
            // ==========================================
            modelBuilder.Entity<Organiser>(entity =>
            {
                entity.ToTable("ORGANISER");
                entity.HasKey(o => o.OrganiserId);
                entity.HasIndex(o => o.Email).IsUnique();

                entity.Property(o => o.Name).IsRequired().HasMaxLength(100);
                entity.Property(o => o.Email).IsRequired().HasMaxLength(150);
                entity.Property(o => o.PasswordHash).IsRequired();
                entity.Property(o => o.Phone).HasMaxLength(20);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ==========================================
            // PARTICIPANT Entity Configuration
            // ==========================================
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("PARTICIPANT");
                entity.HasKey(p => p.ParticipantId);
                entity.HasIndex(p => p.Email).IsUnique();

                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(150);
                entity.Property(p => p.PasswordHash).IsRequired();
                entity.Property(p => p.Gender).HasMaxLength(10);
                entity.Property(p => p.DateOfBirth).HasColumnType("date").IsRequired();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // ==========================================
            // EVENT Entity Configuration
            // ==========================================
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("EVENT");
                entity.HasKey(e => e.EventId);

                entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EventDate).HasColumnType("date").IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(150);

                // Organiser (1) -> Events (Many)
                entity.HasOne(e => e.Organiser)
                      .WithMany(o => o.Events)
                      .HasForeignKey(e => e.OrganiserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // CATEGORY Entity Configuration
            // ==========================================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORY");
                entity.HasKey(c => c.CategoryId);

                entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Discipline).IsRequired().HasMaxLength(20);
                entity.Property(c => c.DistanceKm).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(c => c.EntryFee).HasColumnType("decimal(10,2)").HasDefaultValue(0.00m);

                // Event (1) -> Categories (Many)
                entity.HasOne(c => c.Event)
                      .WithMany(e => e.Categories)
                      .HasForeignKey(c => c.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==========================================
            // REGISTRATION Entity Configuration
            // ==========================================
            modelBuilder.Entity<Registration>(entity =>
            {
                entity.ToTable("REGISTRATION");
                entity.HasKey(r => r.RegistrationId);

                entity.Property(r => r.PaymentStatus).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
                entity.Property(r => r.RegistrationDate).HasDefaultValueSql("GETDATE()");

                // Prevent duplicate registrations for same participant + event
                entity.HasIndex(r => new { r.ParticipantId, r.EventId }).IsUnique();

                entity.HasOne(r => r.Participant)
                      .WithMany(p => p.Registrations)
                      .HasForeignKey(r => r.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Event)
                      .WithMany(e => e.Registrations)
                      .HasForeignKey(r => r.EventId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Category)
                      .WithMany(c => c.Registrations)
                      .HasForeignKey(r => r.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // RESULT Entity Configuration
            // ==========================================
            modelBuilder.Entity<Result>(entity =>
            {
                entity.ToTable("RESULT");
                entity.HasKey(res => res.ResultId);

                entity.Property(res => res.RaceNumber).IsRequired();
                entity.Property(res => res.CreatedAt).HasDefaultValueSql("GETDATE()");

                // 1-to-1 Relationship with Registration
                entity.HasIndex(res => res.RegistrationId).IsUnique();

                entity.HasOne(res => res.Registration)
                      .WithOne(r => r.Result)
                      .HasForeignKey<Result>(res => res.RegistrationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

