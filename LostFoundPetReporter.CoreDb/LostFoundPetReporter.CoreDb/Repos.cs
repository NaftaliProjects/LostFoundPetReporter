
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace LostFoundPetReporter.CoreDb.Repos
{
    
    public abstract class BaseRepo<T> :  IBaseRepo<T> where T : BaseModel
    {
        protected DbSet<T> Table { get; }
        public PetReporterContext Context { get; }
        public int LimitResult { get; set; } = 100;


        protected BaseRepo(PetReporterContext context) 
        {
            Context = context;
            Table = Context.Set<T>();
        }

        protected IQueryable<T> ApplyLimit(IQueryable<T> query)
        {
            if (LimitResult > 0)
                query = query.Take(LimitResult);

            return query;
        }


        public virtual IQueryable<T> GetAll() => ApplyLimit(Table.OrderByDescending(x => x.Id));


        public virtual IQueryable<T> GetAllIgnoreQueryFillters() => ApplyLimit(Table.IgnoreQueryFilters().OrderByDescending(x => x.Id));

        public virtual T Find(int? id) => Table.Find(id)!;
        public virtual T FindAsNoTracking(int id) => Table.AsNoTracking().FirstOrDefault(x => x.Id == id)!;
        public virtual T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id)!;

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
    }






    //Entity-Specific Repositories Imp
    public class UserDeviceRepo : BaseRepo<UserDevice>, IUserDeviceRepo
    {
        public UserDeviceRepo(PetReporterContext context) : base(context) { }

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

        public User GetByEmail(string email) => ApplyLimit(Table.Where(u => u.Email == email)).FirstOrDefault()!;

    }



    public class ReportRepo : BaseRepo<Report>, IReportRepo
    {
        public ReportRepo(PetReporterContext context) : base(context)
        {
        }


        internal IQueryable<Report> ApplayFullInclude(IQueryable<Report> query)
            => query.Include(x => x.ReportExtFilesNevigation).Include(c => c.ReportCoordinateNavigation)

        internal IQueryable<Report> ApplpyReportType(string type, IQueryable<Report> query)
        {
            if (type.IsNullOrEmpty())
                return query;

            if (type.ToLower().Equals("l"))
                return query.Where(r => r.ReportType.ToLower().Equals("l"));
            else
                return query.Where(r => r.ReportType.ToLower().Equals("f"));
        }

        internal IOrderedQueryable<Report> BuildBaseQuey(string type)
        {
            
            if (type.IsNullOrEmpty())
                return Table.OrderBy(r => r.DateReported);
            else
                return ApplpyReportType(type,ApplayFullInclude(Table)).OrderBy(r => r.DateReported);
        }

        public  IOrderedQueryable<Report> GetAll(string type) => BuildBaseQuey(type).Base.GetAll();

        public override IOrderedQueryable<Report> GetAllIgnoreQueryFillters() => ApplayFullInclude(base.GetAllIgnoreQueryFillters());

        public IOrderedQueryable<Report> GetAllByUserId(int userId) => ApplayFullInclude(ApplyLimit(Table.Where(u => u.UserId == userId)));

        public IQueryable<Report> GetReportsInDateRange(DateTime fromDate, DateTime toDate)
        {
            return GetAll().Where(x => x.DateReported >= fromDate && x.DateReported <= toDate);
        }


    }






    public class FoundReportRepo : BaseRepo<FoundReport>, IFoundReportRepo
    {
        public FoundReportRepo(PetReporterContext context) : base(context)
        {
        }


        internal IOrderedQueryable<FoundReport> ApplayFullInclude(IQueryable<FoundReport> query)
            => query.Include(x => x.FoundReportExtFilesNevigation).Include(c => c.FoundCoordinateNavigation).OrderBy(o=>o.dateTime);

       
        public override IOrderedQueryable<FoundReport> GetAll() => ApplayFullInclude(base.GetAll());

        public override IOrderedQueryable<FoundReport> GetAllIgnoreQueryFillters() => ApplayFullInclude(base.GetAllIgnoreQueryFillters());

        public IOrderedQueryable<FoundReport> GetAllByUserId(int userId) => ApplayFullInclude(ApplyLimit(Table.Where(u => u.UserId == userId)));

        public IQueryable<FoundReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate)
        {
            return GetAll().Where(x => x.dateTime >= fromDate && x.dateTime <= toDate);
        }


    }


    public class LostReportRepo : BaseRepo<LostReport>, ILostReportRepo
    {
        public LostReportRepo(PetReporterContext context) : base(context)
        {
        }


        internal IOrderedQueryable<FoundReport> ApplayFullInclude(IQueryable<FoundReport> query)
            => query.Include(x => x.FoundReportExtFilesNevigation).Include(c => c.FoundCoordinateNavigation).OrderBy(o => o.dateTime);



        internal IOrderedQueryable<LostReport> ApplayMinimalInclude(IQueryable<LostReport> query)
            => query
                .Include(x => x.LostReportExtFilesNevigation)
                .Include(x => x.User)
                .OrderBy(x => x.dateTime);

                

        internal IQueryable<LostReport> ApplayFullInclude(IQueryable<LostReport> table)
        => table
            .Include(x => x.User)
            .Include(x => x.LostReportExtFilesNevigation)
            .Include(c => c.LostCoordinateNavigation)
            .Include(x => x.LostFoundMatchNevigation)
                .ThenInclude(x => x.FoundReportNevigation)
                .ThenInclude(x => x.FoundCoordinateNavigation)
            .Include(x => x.LostFoundMatchNevigation)
                .ThenInclude(x => x.FoundReportNevigation)
                .ThenInclude(x => x.FoundReportExtFilesNevigation);

        public override LostReport Find(int? id) => ApplayFullInclude(Table).FirstOrDefault(x => x.Id == id!.Value)!;

        public override IOrderedQueryable<LostReport> GetAll() => ApplayMinimalInclude(base.GetAll());

        public override IOrderedQueryable<LostReport> GetAllIgnoreQueryFillters() => ApplayMinimalInclude(base.GetAllIgnoreQueryFillters());

        public IQueryable<LostReport> GetAllByUserId(int userId) => ApplayMinimalInclude(ApplyLimit(Table.Where(u => u.UserId == userId)));


        public IQueryable<LostReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate)
            => ApplayMinimalInclude(Table.Where(x => x.dateTime >= fromDate && x.dateTime <= toDate)).OrderBy(x => x.dateTime);
        
    }


    public class LostFoundMatchRepo : BaseRepo<LostFoundMatch>, ILostFoundMatchRepo
    {
        public LostFoundMatchRepo(PetReporterContext context) : base(context)
        {
        }



        protected virtual IQueryable<LostFoundMatch> ApplyFullInclude(IQueryable<LostFoundMatch> query)
        {
            return query
                .Include(m => m.LostReportNevigation)
                    .ThenInclude(l => l.LostReportExtFilesNevigation)
                .Include(m => m.FoundReportNevigation)
                    .ThenInclude(f => f.FoundReportExtFilesNevigation);
        }

        

        public override IQueryable<LostFoundMatch> GetAll()
            => ApplyFullInclude(base.GetAll());

        public override IQueryable<LostFoundMatch> GetAllIgnoreQueryFillters()
            => ApplyFullInclude(base.GetAllIgnoreQueryFillters());

        public override LostFoundMatch Find(int? id)
            => ApplyFullInclude(Table).FirstOrDefault(x => x.Id == id!.Value)!;

        public IQueryable<LostFoundMatch> GetByLostReportId(int lostReportId)
            => ApplyFullInclude(Table.Where(m => m.LostReportId == lostReportId));



        public IQueryable<LostFoundMatch> GetByFoundReportId(int foundReportId)
            => ApplyFullInclude(Table.Where(m => m.FoundReportId == foundReportId));

              

        public bool MatchExists(int lostReportId, int foundReportId)
            => Table.Any(m => m.LostReportId == lostReportId && m.FoundReportId == foundReportId);


        public IQueryable<int> GetFoundReportIdsForLostReport(int lostReportId)
            => base.GetAll().Where(m => m.LostReportId == lostReportId).Select(m => m.FoundReportId);


        public IQueryable<int> GetLostReportIdsForFoundReport(int foundReportId)
            => base.GetAll().Where(m => m.FoundReportId == foundReportId).Select(m => m.LostReportId);

    }
}
