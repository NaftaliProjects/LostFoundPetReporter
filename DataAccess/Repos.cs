
using Microsoft.EntityFrameworkCore;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;


namespace LostFoundPetReporter.CoreDb.Repos
{
    

    public abstract class BaseViewRepo<T> : IBaseViewRepo<T> where T : class
    {

        private readonly bool _disoposeContext;
        public PetReporterContext Context { get; }
        protected DbSet<T> Table { get; }

        protected BaseViewRepo(PetReporterContext context)
        {
            Context = context;
            Table = Context.Set<T>();
            _disoposeContext = false;
        }

        protected BaseViewRepo(DbContextOptions<PetReporterContext> options) : this(new PetReporterContext(options))
        {
            _disoposeContext = true;
        }
        public virtual void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDissposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_isDissposed) 
            {
                return;
            }

            if (disposing)
            {
                if (_disoposeContext)
                {
                    Context.Dispose();
                }
            }
            _isDissposed = true;
        }
        ~BaseViewRepo()
        {
            Dispose(false); 
        }

        public virtual IEnumerable<T> ExcuteSqlString(string sql) 
            => Table.FromSqlRaw(sql);
        public virtual IEnumerable<T> GetAll() 
            => Table.AsQueryable();

        public virtual IEnumerable<T> GetAllIgnoreQueryFillters() 
            => Table.AsQueryable().IgnoreQueryFilters();

    }




    public abstract class BaseRepo<T> : BaseViewRepo<T>, IBaseRepo<T> where T : BaseModel
    {
        protected BaseRepo(PetReporterContext context) : base(context) { }
        protected BaseRepo(DbContextOptions<PetReporterContext> options) : this(new PetReporterContext(options)) 
        {
        }

        public int SaveChanges()
        {
            try 
            {
                return Context.SaveChanges();  
            }
            catch (Exception ex) 
            { 
                throw new Exception("An error occurred updating the database", ex); 
            }
        }

        public virtual T? Find(int? id) => Table.Find(id)!;
        public virtual T FindAsNoTracking(int id)
            => Table.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.Id == id)!;
        public virtual T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id)!;
        public virtual void ExecuteQueryFilters(int id) => Table.FirstOrDefault(x => x.Id == id);
        public virtual void ExecutePatameterizedQuery(string sql, object[] sqlParametersObjects)
            => Context.Database.ExecuteSqlRaw(sql, sqlParametersObjects);
        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }
        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }
        public virtual int Update(T entity,T updatedEntity, bool persist = true)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var updatedValue = property.GetValue(updatedEntity);

                if (updatedValue == null)
                    continue;

                if (updatedValue is string stringValue &&
                    string.IsNullOrWhiteSpace(stringValue))
                    continue;

                property.SetValue(entity, updatedValue);
            }

            Table.Update(entity);

            return persist ? SaveChanges() : 0;
        }


        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);

            return persist ? SaveChanges() : 0;
        }
        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0; 
        }
        public virtual int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }
        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }
        public virtual int Delete(int id, bool persist = true)
        {
            var entity = Table.Find(id);

            if (entity == null)
                return 0;

            Table.Remove(entity);

            return persist ? SaveChanges() : 0;
        }
    }






    //Entity-Specific Repositories Imp


    public class UserDeviceRepo : BaseRepo<UserDevice>, IUserDeviceRepo
    {
        public UserDeviceRepo(PetReporterContext context)
            : base(context)
        {
        }

        internal UserDeviceRepo(
            DbContextOptions<PetReporterContext> options)
            : base(options)
        {
        }

        public UserDevice? GetByUserId(int userId)
        {
            return Table.FirstOrDefault(x => x.UserId == userId);
        }

        public UserDevice? GetByToken(string token)
        {
            return Table.FirstOrDefault(x => x.Token == token);
        }

        public void RegisterDevice(UserDevice device)
        {
            var existingDevice = GetByUserId(device.UserId);

            if (existingDevice == null)
            {
                Add(device);
                return;
            }

            existingDevice.Token = device.Token;
            existingDevice.Platform = device.Platform;
            existingDevice.LastUpdated = device.LastUpdated;

            Update(existingDevice);
        }
    }



    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        public UserRepo(PetReporterContext context) : base(context)
        {
        }

        internal UserRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }

        public override IEnumerable<User> GetAll()
            => Table.OrderBy(u => u.Name);
        public override IEnumerable<User> GetAllIgnoreQueryFillters()
            => Table.OrderBy(u => u.Name);
        public  IEnumerable<User> GetAllByUserId(int id)
            => Table.Where(u => u.Id == id);

        public override User Find(int? id)
            => Table
                .IgnoreQueryFilters()
                .Where(x => x.Id == id)
                .FirstOrDefault()!;

        public User GetByEmail(string email)
            => Table.FirstOrDefault(u => u.Email == email)!;


    }


    public class ReportRepo : BaseRepo<Report>, IReportRepo
    {
        private readonly string _type;

        public ReportRepo(PetReporterContext context, string type)
            : base(context)
        {
            _type = type;
        }

        internal ReportRepo(
            DbContextOptions<PetReporterContext> options,
            string type)
            : base(options)
        {
            _type = type;
        }

        internal IQueryable<Report> BuildBaseQuery()
            => Table
                .Include(r => r.ReportExtFilesNevigation)
                .Include(r => r.ReportCoordinateNavigation)
                .Include(r => r.UserNevigation)
                .Where(r => r.ReportType == _type)
                .OrderBy(r => r.DateReported);

        internal IQueryable<Report> BuildDetailsQuery()
            => Table
                .Include(r => r.UserNevigation)
                .Include(r => r.ReportExtFilesNevigation)
                .Include(r => r.ReportCoordinateNavigation)

                .Include(r => r.LostMatches)
                    .ThenInclude(m => m.FoundReportNevigation)
                        .ThenInclude(found => found.ReportCoordinateNavigation)

                .Include(r => r.LostMatches)
                    .ThenInclude(m => m.FoundReportNevigation)
                        .ThenInclude(found => found.ReportExtFilesNevigation)

                .Include(r => r.FoundMatches)
                    .ThenInclude(m => m.LostReportNevigation)
                        .ThenInclude(lost => lost.ReportCoordinateNavigation)

                .Include(r => r.FoundMatches)
                    .ThenInclude(m => m.LostReportNevigation)
                        .ThenInclude(lost => lost.ReportExtFilesNevigation);

        public override IEnumerable<Report> GetAll()
            => BuildBaseQuery().ToList();

        public override IEnumerable<Report> GetAllIgnoreQueryFillters()
            => BuildBaseQuery()
                .IgnoreQueryFilters()
                .ToList();

        public IEnumerable<Report> GetAllBy(int id)
            => BuildBaseQuery()
                .Where(r => r.Id == id)
                .ToList();

        public override Report Find(int? id)
            => BuildBaseQuery()
                .IgnoreQueryFilters()
                .FirstOrDefault(r => r.Id == id)!;

        public Report FindDetailed(int? id)
            => BuildDetailsQuery()
                .IgnoreQueryFilters()
                .Where(r => r.ReportType == _type)
                .FirstOrDefault(r => r.Id == id)!;

        public IEnumerable<Report> GetAllByUserId(int userId)
            => BuildBaseQuery()
                .Where(r => r.UserId == userId)
                .ToList();

        public IEnumerable<Report> GetAllByUserIdDetailed(int userId)
            => BuildDetailsQuery()
                .Where(r => r.UserId == userId)
                .Where(r => r.ReportType == _type)
                .ToList();

        public IEnumerable<Report> GetReportsInDateRange(
            DateTime fromDate,
            DateTime toDate)
            => BuildBaseQuery()
                .Where(r =>
                    r.DateReported >= fromDate &&
                    r.DateReported <= toDate)
                .ToList();
    }


    public class FoundReportRepo : ReportRepo, IFoundReportRepo
    {
        public FoundReportRepo(PetReporterContext context) : base(context,"f")
        {
        }

        internal FoundReportRepo(DbContextOptions<PetReporterContext> options) : base(options,"f")
        {
        }

    }


    public class LostReportRepo : ReportRepo, ILostReportRepo
    {
        public LostReportRepo(PetReporterContext context) : base(context,"l")
        {
        }

        internal LostReportRepo(DbContextOptions<PetReporterContext> options) : base(options,"l")
        {
        }

    }


    public class LostFoundMatchRepo : BaseRepo<LostFoundMatch>, ILostFoundMatchRepo
    {
        public LostFoundMatchRepo(PetReporterContext context) : base(context)
        {
        }

        internal LostFoundMatchRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }


        protected virtual IQueryable<LostFoundMatch> BuildBaseQuery(bool ignoreFilters = false)
        {
            var query = Table.AsQueryable();

            if (ignoreFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            return query
                .Include(m => m.LostReportNevigation)
                    .ThenInclude(l => l.ReportExtFilesNevigation)
                .Include(m => m.FoundReportNevigation)
                    .ThenInclude(f => f.ReportExtFilesNevigation)
                .OrderByDescending(m => m.Id);
        }

        public override IEnumerable<LostFoundMatch> GetAll()
            => BuildBaseQuery().ToList();

        public override IEnumerable<LostFoundMatch> GetAllIgnoreQueryFillters()
            => BuildBaseQuery(ignoreFilters: true).ToList();

        public override LostFoundMatch? Find(int? id)
            => BuildBaseQuery(ignoreFilters: true)
                .FirstOrDefault(m => m.Id == id);

        public IEnumerable<LostFoundMatch> GetByLostReportId(int lostReportId)
            => BuildBaseQuery()
                .Where(m => m.LostReportId == lostReportId)
                .ToList();

        public IEnumerable<LostFoundMatch> GetByFoundReportId(int foundReportId)
            => BuildBaseQuery()
                .Where(m => m.FoundReportId == foundReportId)
                .ToList();

        public bool MatchExists(int lostReportId, int foundReportId)
            => Table.Any(m => m.LostReportId == lostReportId && m.FoundReportId == foundReportId);


        public IEnumerable<int> GetFoundReportIdsForLostReport(int lostReportId)
        {
            return Table
                .Where(m => m.LostReportId == lostReportId)
                .Select(m => m.FoundReportId)
                .ToList();
        }

        public IEnumerable<int> GetLostReportIdsForFoundReport(int foundReportId)
        {
            return Table
                .Where(m => m.FoundReportId == foundReportId)
                .Select(m => m.LostReportId)
                .ToList();
        }


    }
}
