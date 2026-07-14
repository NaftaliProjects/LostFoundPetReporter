using System;
using System.Collections.Generic;
using System.Text;
using static LostFoundPetReporter.CoreDb.Models;


namespace LostFoundPetReporter.CoreDb
{
    public class PetReporterContext : DbContext
    {
        public PetReporterContext(DbContextOptions<PetReporterContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<LostReport> LostReports { get; set; }
        public DbSet<FoundReport> FoundReports { get; set; }
        public DbSet<FoundReportExtFile> FoundReportExtFiles { get; set; }
        public DbSet<LostReportExtFile> LostReportExtFiles { get; set; }

    }



    public class PetReporterContextFactory : IDesignTimeDbContextFactory<PetReporterContext>
    {
        public PetReporterContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<PetReporterContext>();
            var connectionString = @"server=.,5433;Database=LFPR;User Id=sa;Password=P@ssw0rd;Encrypt=False;";
            optionBuilder.UseSqlServer(connectionString);
            return new PetReporterContext(optionBuilder.Options);
        }
    }
}
