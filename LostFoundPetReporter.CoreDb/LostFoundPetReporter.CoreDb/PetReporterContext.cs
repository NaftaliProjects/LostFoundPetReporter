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


            //UserDevice Builder
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




            //User Builder 
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(p => p.Name).HasMaxLength(20);
                entity.Property(p => p.Email).HasMaxLength(30);
                entity.Property(p => p.Phone).HasMaxLength(12);
                entity.Property(p => p.HashedPassword).HasMaxLength(256);


            });


            //Report Builder
            modelBuilder.Entity<Coordinate>()
             .HasKey(x => x.ReportId);

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");
                entity.OwnsOne(lr => lr.PetDescription);    
            });

            modelBuilder.Entity<Report>()
            .HasOne(x => x.ReportCoordinateNavigation)
            .WithOne(x => x.ReportNavigation)
            .HasForeignKey<Coordinate>(
                x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExtFile>(entity =>
            {
                entity.ToTable("ExtFiles");
                entity.Property(p => p.FilePath).HasMaxLength(70);
                entity.Property(p => p.Description).HasMaxLength(30);
                entity.Property(p => p.FileName).HasMaxLength(30);
            });







            //LostFoundMatch Builder
            modelBuilder.Entity<LostFoundMatch>()
            .HasOne(match => match.LostReportNevigation)
            .WithMany(report => report.LostMatches)
            .HasForeignKey(match => match.LostReportId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LostFoundMatch>()
                .HasOne(match => match.FoundReportNevigation)
                .WithMany(report => report.FoundMatches)
                .HasForeignKey(match => match.FoundReportId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var deletedFoundReports = ChangeTracker.Entries<Report>()
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
        public DbSet<Report> Reports { get; set; }
        public DbSet<ExtFile> ExtFiles { get; set; }
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
