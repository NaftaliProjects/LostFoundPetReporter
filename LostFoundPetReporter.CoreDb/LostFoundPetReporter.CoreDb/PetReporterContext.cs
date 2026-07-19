using System;
using System.Collections.Generic;
using System.Text;
using  LostFoundPetReporter.CoreDb.Models;


namespace LostFoundPetReporter.CoreDb
{
    public class PetReporterContext : DbContext
    {
        public PetReporterContext(DbContextOptions<PetReporterContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(p => p.Name).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(30);
                entity.Property(p => p.Phone).HasMaxLength(12);
                entity.Property(p => p.HashedPassword).HasMaxLength(64);


            });


            modelBuilder.Entity<LostReport>(entity =>
            {
                entity.ToTable("LostReports");
                entity.OwnsOne(lr => lr.PetDescription);
                
            });

            modelBuilder.Entity<FoundReport>(entity =>
            {
                entity.ToTable("FoundReports");
                entity.OwnsOne(fr => fr.PetDescription);
            });

            modelBuilder.Entity<FoundReportExtFile>(entity =>
            {
                entity.ToTable("FoundReportExtFiles");
                entity.Property(p => p.FilePath).HasMaxLength(70);
                entity.Property(p => p.Description).HasMaxLength(30);
                entity.Property(p => p.FileName).HasMaxLength(30);
                
            });

            modelBuilder.Entity<LostReportExtFile>(entity =>
            {
                entity.ToTable("LostReportExtFiles");
                entity.Property(p => p.FilePath).HasMaxLength(70);
                entity.Property(p => p.Description).HasMaxLength(30);
                entity.Property(p => p.FileName).HasMaxLength(30);
            });

            modelBuilder.Entity<LostFoundMatch>()
            .HasOne(m => m.LostReport)
            .WithMany(r => r.Matches)
            .HasForeignKey(m => m.LostReportId);

            modelBuilder.Entity<LostFoundMatch>()
            .HasOne(m => m.FoundReport)
            .WithMany(r => r.Matches)
            .HasForeignKey(m => m.FoundReportId)
            .OnDelete(DeleteBehavior.NoAction);

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedFoundReports = ChangeTracker.Entries<FoundReport>()
                .Where(e => e.State == EntityState.Deleted)
                .Select(e => e.Entity)
                .ToList();

            foreach (var foundReport in deletedFoundReports)
            {
                var relatedMatches = LostFoundMatches
                    .Where(m => m.FoundReportId == foundReport.Id);

                LostFoundMatches.RemoveRange(relatedMatches);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LostReport> LostReports { get; set; }
        public DbSet<FoundReport> FoundReports { get; set; }
        public DbSet<FoundReportExtFile> FoundReportExtFiles { get; set; }
        public DbSet<LostReportExtFile> LostReportExtFiles { get; set; }
        public DbSet<LostFoundMatch> LostFoundMatches { get; set; }

    }



    public class PetReporterContextFactory : IDesignTimeDbContextFactory<PetReporterContext>
    {
        public PetReporterContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<PetReporterContext>();
            var connectionString = @"Server=localhost\SQLEXPRESS01;Database=LFPR;Trusted_Connection=True;TrustServerCertificate=True;";
            optionBuilder.UseSqlServer(connectionString);
            return new PetReporterContext(optionBuilder.Options);
        }
    }
}
