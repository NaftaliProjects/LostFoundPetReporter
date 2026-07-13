using System;
using System.Collections.Generic;
using System.Text;


namespace LostFoundPetReporter.CoreDb
{
    public class PetReporterContext : DbContext
    {
        public PetReporterContext(DbContextOptions<PetReporterContextContext> options) : base(options)
        {

        }
    }



    public class PetReporterContextFactory : IDesignTimeDbContextFactory<PetReporterContext>
    {
        public PetReporterContext CreateDbContext(stringp[] args)
        {
            var optionBuilder = new DbContextOptionBuilder<PetReporterContext>();
            var connectionString = @"server=.,5433;Database=LFPR;User Id=sa;Password=P@ssw0rd;Encrypt=False;";
            optionBuilder.UseSqlServer(connectionString);
            return new PetReporterContextFactory(optionBuilder.Options);
        }
    }
}
