
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.EntityFrameworkCore;

namespace LostFoundPetReporter.CoreDb.Repos
{
    // =========================================================
    // Base Repository
    // =========================================================

    public abstract class BaseRepo<T> : IBaseRepo<T>
        where T : BaseModel
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
            return LimitResult > 0
                ? query.Take(LimitResult)
                : query;
        }

        // Query builders: derived repositories can customize these.
        protected virtual IQueryable<T> BuildGetAllQuery()
            => Table.OrderByDescending(x => x.Id);

        protected virtual IQueryable<T> BuildGetAllIgnoreQueryFiltersQuery()
            => Table.IgnoreQueryFilters()
                     .OrderByDescending(x => x.Id);

        // Public read methods: execute queries before returning.
        public virtual IEnumerable<T> GetAll()
            => ApplyLimit(BuildGetAllQuery()).ToList();

        public virtual IEnumerable<T> GetAllIgnoreQueryFillters()
            => ApplyLimit(BuildGetAllIgnoreQueryFiltersQuery())
                .ToList();

        public virtual T Find(int? id)
            => Table.Find(id)!;

        public virtual T FindAsNoTracking(int id)
            => Table.AsNoTracking()
                    .FirstOrDefault(x => x.Id == id)!;

        public virtual T FindIgnoreQueryFilters(int id)
            => Table.IgnoreQueryFilters()
                    .FirstOrDefault(x => x.Id == id)!;

        // Create
        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int AddRange(
            IEnumerable<T> entities,
            bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }

        // Update using non-null/non-empty values from updatedEntity.
        public virtual int Update(
            T entity,
            T updatedEntity,
            bool persist = true)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                // Never update the primary key through this method.
                if (property.Name == nameof(BaseModel.Id))
                    continue;

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

        public virtual int UpdateRange(
            IEnumerable<T> entities,
            bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }

        // Delete
        public virtual int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int DeleteRange(
            IEnumerable<T> entities,
            bool persist = true)
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

        // Persistence
        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An error occurred updating the database", ex);
            }
        }
    }


    // =========================================================
    // User Device Repository
    // =========================================================

    public class UserDeviceRepo
        : BaseRepo<UserDevice>, IUserDeviceRepo
    {
        public UserDeviceRepo(PetReporterContext context)
            : base(context)
        {
        }

        public UserDevice? GetByUserId(int userId)
            => Table.FirstOrDefault(x => x.UserId == userId);

        public UserDevice? GetByToken(string token)
            => Table.FirstOrDefault(x => x.Token == token);

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


    // =========================================================
    // User Repository
    // =========================================================

    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        public UserRepo(PetReporterContext context)
            : base(context)
        {
        }

        public User? GetByEmail(string email)
            => Table.FirstOrDefault(u => u.Email == email);
    }


    // =========================================================
    // Report Repository
    // Responsible for report type and report-specific filters.
    // =========================================================

    public class ReportRepo : BaseRepo<Report>, IReportRepo
    {
        private readonly string _type;

        public ReportRepo(PetReporterContext context, string type)
            : base(context)
        {
            _type = type;
        }

        protected override IQueryable<Report> BuildGetAllQuery()
            => base.BuildGetAllQuery()
                .Where(r => r.ReportType == _type);

        protected override IQueryable<Report>
            BuildGetAllIgnoreQueryFiltersQuery()
            => base.BuildGetAllIgnoreQueryFiltersQuery()
                .Where(r => r.ReportType == _type);

        protected virtual IQueryable<Report> BuildGetAllByUserIdQuery(
            int userId)
            => Table.Where(r =>
                    r.UserId == userId &&
                    r.ReportType == _type)
                .OrderByDescending(r => r.Id);

        protected virtual IQueryable<Report>
            BuildGetReportsInDateRangeQuery(
                DateTime fromDate,
                DateTime toDate)
            => Table.Where(r =>
                    r.ReportType == _type &&
                    r.DateReported >= fromDate &&
                    r.DateReported <= toDate)
                .OrderByDescending(r => r.Id);

        public virtual IEnumerable<Report> GetAllByUserId(int userId)
            => ApplyLimit(
                BuildGetAllByUserIdQuery(userId))
                .ToList();

        public virtual IEnumerable<Report> GetReportsInDateRange(
            DateTime fromDate,
            DateTime toDate)
            => ApplyLimit(
                BuildGetReportsInDateRangeQuery(fromDate, toDate))
                .ToList();

        public override Report Find(int? id)
        {
            if (!id.HasValue)
                return null!;

            return Table.FirstOrDefault(r =>
                r.Id == id.Value &&
                r.ReportType == _type)!;
        }

        public override Report FindAsNoTracking(int id)
    => Table
        .AsNoTracking()
        .FirstOrDefault(r =>
            r.Id == id &&
            r.ReportType == _type)!;

        public override Report FindIgnoreQueryFilters(int id)
            => Table
                .IgnoreQueryFilters()
                .FirstOrDefault(r =>
                    r.Id == id &&
                    r.ReportType == _type)!;
    }


    // =========================================================
    // Found Report Repository
    // Responsible only for loading found-report navigation data.
    // =========================================================

    public class FoundReportRepo
        : ReportRepo, IFoundReportRepo
    {
        public FoundReportRepo(PetReporterContext context)
            : base(context, "f")
        {
        }

        protected override IQueryable<Report> BuildGetAllQuery()
            => base.BuildGetAllQuery()
                .Include(x => x.ReportExtFilesNevigation)
                .Include(x => x.ReportCoordinateNavigation);

        protected override IQueryable<Report>
            BuildGetAllIgnoreQueryFiltersQuery()
            => base.BuildGetAllIgnoreQueryFiltersQuery()
                .Include(x => x.ReportExtFilesNevigation)
                .Include(x => x.ReportCoordinateNavigation);




    }


    // =========================================================
    // Lost Report Repository
    // Adds minimal Includes for lists and full Includes for Find.
    // =========================================================

    public class LostReportRepo
        : ReportRepo, ILostReportRepo
    {
        public LostReportRepo(PetReporterContext context)
            : base(context, "l")
        {
        }

        private IQueryable<Report> ApplyMinimalIncludes(
            IQueryable<Report> query)
            => query
                .Include(x => x.ReportExtFilesNevigation)
                .Include(x => x.UserNevigation);

        private IQueryable<Report> ApplyFullIncludes(
            IQueryable<Report> query)
            => query
                .Include(x => x.UserNevigation)
                .Include(x => x.ReportExtFilesNevigation)
                .Include(x => x.ReportCoordinateNavigation)
                .Include(x => x.LostFoundMatchNevigation)
                    .ThenInclude(x => x.LostReportNevigation)
                    .ThenInclude(x => x.ReportCoordinateNavigation)
                .Include(x => x.LostFoundMatchNevigation)
                    .ThenInclude(x => x.FoundReportNevigation)
                    .ThenInclude(x => x.ReportExtFilesNevigation);

        protected override IQueryable<Report> BuildGetAllQuery()
            => ApplyMinimalIncludes(base.BuildGetAllQuery());

        protected override IQueryable<Report>
            BuildGetAllIgnoreQueryFiltersQuery()
            => ApplyMinimalIncludes(
                base.BuildGetAllIgnoreQueryFiltersQuery());

        protected override IQueryable<Report>
            BuildGetAllByUserIdQuery(int userId)
            => ApplyMinimalIncludes(
                base.BuildGetAllByUserIdQuery(userId));

        protected override IQueryable<Report>
            BuildGetReportsInDateRangeQuery(
                DateTime fromDate,
                DateTime toDate)
            => ApplyMinimalIncludes(
                base.BuildGetReportsInDateRangeQuery(
                    fromDate, toDate));

        public override Report Find(int? id)
        {
            if (!id.HasValue)
                return null!;

            return ApplyFullIncludes(
                    Table.Where(x =>
                        x.Id == id.Value &&
                        x.ReportType == "l"))
                .FirstOrDefault()!;
        }
    }


    // =========================================================
    // Lost/Found Match Repository
    // =========================================================

    public class LostFoundMatchRepo
        : BaseRepo<LostFoundMatch>, ILostFoundMatchRepo
    {
        public LostFoundMatchRepo(PetReporterContext context)
            : base(context)
        {
        }

        protected IQueryable<LostFoundMatch> ApplyFullIncludes(
            IQueryable<LostFoundMatch> query)
            => query
                .Include(m => m.LostReportNevigation)
                    .ThenInclude(l => l.ReportExtFilesNevigation)
                .Include(m => m.FoundReportNevigation)
                    .ThenInclude(f => f.ReportExtFilesNevigation);

        protected override IQueryable<LostFoundMatch> BuildGetAllQuery()
            => ApplyFullIncludes(base.BuildGetAllQuery());

        protected override IQueryable<LostFoundMatch>
            BuildGetAllIgnoreQueryFiltersQuery()
            => ApplyFullIncludes(
                base.BuildGetAllIgnoreQueryFiltersQuery());

        public override LostFoundMatch Find(int? id)
        {
            if (!id.HasValue)
                return null!;

            return ApplyFullIncludes(
                    Table.Where(x => x.Id == id.Value))
                .FirstOrDefault()!;
        }

        public IEnumerable<LostFoundMatch> GetByLostReportId(
            int lostReportId)
            => ApplyLimit(
                ApplyFullIncludes(
                    Table.Where(m => m.LostReportId == lostReportId)))
                .ToList();

        public IEnumerable<LostFoundMatch> GetByFoundReportId(
            int foundReportId)
            => ApplyLimit(
                ApplyFullIncludes(
                    Table.Where(m => m.FoundReportId == foundReportId)))
                .ToList();

        public bool MatchExists(
            int lostReportId,
            int foundReportId)
            => Table.Any(m =>
                m.LostReportId == lostReportId &&
                m.FoundReportId == foundReportId);

        public IEnumerable<int> GetFoundReportIdsForLostReport(
            int lostReportId)
        {
            var query = Table
                .Where(m => m.LostReportId == lostReportId)
                .Select(m => m.FoundReportId);

            return LimitResult > 0
                ? query.Take(LimitResult).ToList()
                : query.ToList();
        }

        public IEnumerable<int> GetLostReportIdsForFoundReport(
            int foundReportId)
        {
            var query = Table
                .Where(m => m.FoundReportId == foundReportId)
                .Select(m => m.LostReportId);

            return LimitResult > 0
                ? query.Take(LimitResult).ToList()
                : query.ToList();
        }
    }
}
