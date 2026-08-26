
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;


namespace LostFoundPetReporter.CoreDb.Repos
{
    

    public abstract class BaseViewRepo<T> : IBaseViewRepo<T> where T : class, new()
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




    public abstract class BaseRepo<T> : BaseViewRepo<T>, IBaseRepo<T> where T : BaseModel, new()
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

        public virtual T Find(int? id) => Table.Find(id);
        public virtual T FindAsNoTracking(int id)
            => Table.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.Id == id);
        public virtual T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
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
        public int Delete(int id, bool persist = true)
        {
            var entity = new T { Id = id };
            Context.Entry(entity).State = EntityState.Deleted;
            return persist ? SaveChanges() : 0;
        }
    }

    //Entity-Specific Repositories Imp
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
                .FirstOrDefault();

        public User GetByEmail(string email)
            => Table.FirstOrDefault(u => u.Email == email);


    }



    public class FoundReportRepo : BaseRepo<FoundReport>, IFoundReportRepo
    {
        public FoundReportRepo(PetReporterContext context) : base(context)
        {
        }

        internal FoundReportRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }

        internal IOrderedQueryable<FoundReport> BuildBaseQuery()
            => Table.Include(x => x.FoundReportExtFilesNevigation).Include(c => c.FoundCoordinateNavigation).OrderBy(o=>o.dateTime);

        public override IEnumerable<FoundReport> GetAll()
            => BuildBaseQuery();
        public override IEnumerable<FoundReport> GetAllIgnoreQueryFillters()
            => BuildBaseQuery().IgnoreQueryFilters();
        public IEnumerable<FoundReport> GetAllBy(int id)
            => Table.Where(u => u.Id == id);

        public override FoundReport Find(int? id)
            => Table
                .IgnoreQueryFilters()
                .Where(x => x.Id == id)
                .FirstOrDefault();

        public IEnumerable<FoundReport> GetAllByUserId(int userId)
            => BuildBaseQuery().Where(u => u.UserId == userId);


    }


    public class LostReportRepo : BaseRepo<LostReport>, ILostReportRepo
    {
        public LostReportRepo(PetReporterContext context) : base(context)
        {
        }

        internal LostReportRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }

        
         internal IOrderedQueryable<LostReport> BuildBaseQuery()
            => Table
                .Include(x => x.LostReportExtFilesNevigation)
                .Include(x => x.User)
                .OrderBy(x => x.dateTime);

        internal IQueryable<LostReport> BuildDetailsQuery()
        => Table
            .Include(x => x.User)
            .Include(x => x.LostReportExtFilesNevigation)
            .Include(c => c.LostCoordinateNavigation)
            .Include(x => x.LostFoundMatchNevigation)
                .ThenInclude(x => x.FoundReportNevigation)
                .ThenInclude(x => x.FoundCoordinateNavigation);

        public override IEnumerable<LostReport> GetAll()
            => BuildBaseQuery();
        public override IEnumerable<LostReport> GetAllIgnoreQueryFillters()
            => BuildBaseQuery().IgnoreQueryFilters();
        public IEnumerable<LostReport> GetAllBy(int id)
            => Table.Where(u => u.Id == id);

        public override LostReport Find(int? id)
            => BuildDetailsQuery()
                .IgnoreQueryFilters()
                .Where(x => x.Id == id)
                .FirstOrDefault();

        public IEnumerable<LostReport> GetAllByUserId(int userId)
            => BuildDetailsQuery().Where(u => u.UserId == userId);
    }


    public class LostFoundMatchRepo : BaseRepo<LostFoundMatch>, ILostFoundMatchRepo
    {
        public LostFoundMatchRepo(PetReporterContext context) : base(context)
        {
        }

        internal LostFoundMatchRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }

        /// <summary>
        /// Builds the base query with eager loading for both Lost and Found reports 
        /// and their navigation properties.
        /// </summary>
        protected virtual IQueryable<LostFoundMatch> BuildBaseQuery(bool ignoreFilters = false)
        {
            var query = Table.AsQueryable();

            if (ignoreFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            return query
                // Include LostReport and its external files
                .Include(m => m.LostReportNevigation)
                    .ThenInclude(l => l.LostReportExtFilesNevigation)
                // Include FoundReport and its external files
                .Include(m => m.FoundReportNevigation)
                    .ThenInclude(f => f.FoundReportExtFilesNevigation)
                .OrderByDescending(m => m.Id); // Or order by a CreatedDate if present
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
    }
}
