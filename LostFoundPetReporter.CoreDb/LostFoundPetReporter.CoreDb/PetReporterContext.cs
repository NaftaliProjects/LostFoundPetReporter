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
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("UserDevices");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Token)
                    .IsRequired();

                entity.Property(x => x.Platform)
                    .IsRequired();

                entity.Property(x => x.LastUpdated)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<UserDevice>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.Token)
                    .IsUnique();
            });




            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(p => p.Name).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(30);
                entity.Property(p => p.Phone).HasMaxLength(12);
                entity.Property(p => p.HashedPassword).HasMaxLength(64);


            });

            modelBuilder.Entity<LostCoordinate>()
             .HasKey(x => x.LostReportId);

            modelBuilder.Entity<LostReport>(entity =>
            {
                entity.ToTable("LostReports");
                entity.OwnsOne(lr => lr.PetDescription);    
            });

            modelBuilder.Entity<LostReport>()
            .HasOne(x => x.LostCoordinateNavigation)
            .WithOne(x => x.LostReportNavigation)
            .HasForeignKey<LostCoordinate>(
                x => x.LostReportId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<FoundCoordinate>()
               .HasKey(x => x.FoundReportId);


            modelBuilder.Entity<FoundReport>(entity =>
            {
                entity.ToTable("FoundReports");
                entity.OwnsOne(fr => fr.PetDescription);
            });

            modelBuilder.Entity<FoundReport>()
            .HasOne(x => x.FoundCoordinateNavigation)
            .WithOne(x => x.FoundReportNavigation)
            .HasForeignKey<FoundCoordinate>(
                x => x.FoundReportId)
            .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<LostFoundMatch>(builder =>
            {
                builder.HasIndex(m => new { m.LostReportId, m.FoundReportId })
                       .IsUnique();

                builder.HasOne(m => m.LostReportNevigation)
                       .WithMany(r => r.LostFoundMatchNevigation)
                       .HasForeignKey(m => m.LostReportId)
                       .OnDelete(DeleteBehavior.Cascade); 

                builder.HasOne(m => m.FoundReportNevigation)
                       .WithMany(r => r.LostFoundMatchNevigation)
                       .HasForeignKey(m => m.FoundReportId)
                       .OnDelete(DeleteBehavior.NoAction); 
            });

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

        public DbSet<UserDevice> UserDevice { get; set; }
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
            //var connectionString = @"Server=localhost\SQLEXPRESS01;Database=LFPR;Trusted_Connection=True;TrustServerCertificate=True;";
            var connectionString = @"Server=localhost\NaftulENV;Database=LFPR;Trusted_Connection=True;TrustServerCertificate=True;";
            optionBuilder.UseSqlServer(connectionString);
            return new PetReporterContext(optionBuilder.Options);
        }
    }
}
