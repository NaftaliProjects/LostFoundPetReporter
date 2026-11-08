using System.Text.Json;
using LostFoundPetReporter.CoreDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;

namespace LostFoundPetReporter.CoreDb
{
    public class PetReporterContext : DbContext
    {
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ExtFile> ExtFiles { get; set; }
        public DbSet<LostFoundMatch> LostFoundMatches { get; set; }

        public PetReporterContext(DbContextOptions<PetReporterContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- User ----------
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
                entity.Property(p => p.HashedPassword).HasMaxLength(256).IsRequired();
                entity.Property(p => p.Email).HasMaxLength(256);
                entity.Property(p => p.Phone).HasMaxLength(32);

                entity.HasIndex(p => p.Email).IsUnique();
            });

            // ---------- UserDevice ----------
            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("UserDevices");

                entity.Property(x => x.Token).HasMaxLength(512).IsRequired();
                entity.Property(x => x.Platform).HasConversion<string>().HasMaxLength(20);
                entity.Property(x => x.LastUpdated).IsRequired();

                entity.HasOne(x => x.UserNavigation)
                      .WithMany(u => u.Devices)
                      .HasForeignKey("UserId")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.Token).IsUnique();
            });



            // ---------- Report ----------
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");

                entity.Property(r => r.Type).HasConversion<string>().HasMaxLength(10);

                // deleting a user keeps their reports (anonymous)
                entity.HasOne(r => r.UserNavigation)
                      .WithMany(u => u.Reports)
                      .HasForeignKey("UserId")
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.OwnsOne(r => r.Coordinate, c =>
                {
                    c.Property(p => p.Latitude).HasColumnName("Latitude");
                    c.Property(p => p.Longitude).HasColumnName("Longitude");
                });

                // Dictionary -> JSON string column
                var comparer = new ValueComparer<IDictionary<string, string>?>(
                    (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null)
                           == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
                    v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                    v => v == null ? null
                         : JsonSerializer.Deserialize<Dictionary<string, string>>(
                               JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                               (JsonSerializerOptions?)null));

                entity.Property(r => r.AnimalDescriptions)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                          s => JsonSerializer.Deserialize<Dictionary<string, string>>(s, (JsonSerializerOptions?)null)!,
                          comparer);
            });

            // ---------- ExtFile ----------
            modelBuilder.Entity<ExtFile>(entity =>
            {
                entity.ToTable("ExtFiles");
                entity.Property(p => p.FilePath).HasMaxLength(500).IsRequired();
                entity.Property(p => p.FileName).HasMaxLength(255).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(500);

                entity.HasOne(f => f.ReportNavigation)
                      .WithMany(r => r.ExtFilesNavigation)
                      .HasForeignKey("ReportId")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- LostFoundMatch ----------
            modelBuilder.Entity<LostFoundMatch>(entity =>
            {
                entity.ToTable("LostFoundMatches");

                entity.Property(m => m.MatchReason).HasMaxLength(1000);
                

                entity.HasOne(m => m.LostReportNavigation)
                      .WithMany(r => r.MatchesAsLost)
                      .HasForeignKey("LostReportId")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.FoundReportNavigation)
                      .WithMany(r => r.MatchesAsFound)
                      .HasForeignKey("FoundReportId")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex("LostReportId", "FoundReportId").IsUnique();
            });
        }

        //remove a deleted report's matches.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedIds = ChangeTracker.Entries<Report>()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity.Id)
                .ToList();

            if (deletedIds.Count > 0)
            {
                var related = await LostFoundMatches
                    .Where(m => deletedIds.Contains(EF.Property<KeyType>(m, "LostReportId"))
                             || deletedIds.Contains(EF.Property<KeyType>(m, "FoundReportId")))
                    .ToListAsync(cancellationToken);

                LostFoundMatches.RemoveRange(related);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    public class PetReporterContextFactory : IDesignTimeDbContextFactory<PetReporterContext>
    {
        public PetReporterContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<PetReporterContext>();
            //var connectionString = @"Server=localhost\SQLEXPRESS01;Database=LFPR;Trusted_Connection=True;TrustServerCertificate=True;";
            var connectionString = @"Server=localhost\NaftulENV;Database=LFPR;Trusted_Connection=True;TrustServerCertificate=True;";
            optionBuilder.UseSqlServer(connectionString);
            return new PetReporterContext(optionBuilder.Options);
        }
    }
}