
using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.CoreDb.ReposInterfaces
{
    public interface IRepositoryFactory
    {
        TRepo CreateRepository<TRepo>() where TRepo : class;
    }



    public interface IBaseRepo<T>  where T : BaseModel
    {
        PetReporterContext Context { get; }
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIgnoreQueryFillters();

        T Find(int? id);
        T FindAsNoTracking(int id);
        T FindIgnoreQueryFilters(int id);
        int Add(T entity, bool persist = true);
        int AddRange(IEnumerable<T> entities, bool persist = true);
        int Update(T entity, bool persist = true);
        int Update(T entity,T updatedEntity, bool persist = true);
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Delete(T entity, bool persist = true);
        int DeleteRange(IEnumerable<T> entities, bool persist = true);
        int SaveChanges();
    }

    public interface IUserDeviceRepo : IBaseRepo<UserDevice>
    {
        UserDevice? GetByUserId(int userId);

        UserDevice? GetByToken(string token);

        void RegisterDevice(UserDevice device);
    }

    public interface IUserRepo : IBaseRepo<User>
    {
        public User? GetByEmail(string email);
    }


    public interface IReportRepo : IBaseRepo<Report>
    {
        IOrderedQueryable<Report> GetAllByUserId(int userId);
        IQueryable<Report> GetReportsInDateRange(DateTime fromDate, DateTime toDate);

    }


    public interface IFoundReportRepo : IBaseRepo<FoundReport>
    {
        IOrderedQueryable<FoundReport> GetAllByUserId(int userId);
        IQueryable<FoundReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate);

    }

    public interface ILostReportRepo : IBaseRepo<LostReport>
    {
        IQueryable<LostReport> GetAllByUserId(int userId);
        IQueryable<LostReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate);
    }

    public interface ILostFoundMatchRepo : IBaseRepo<LostFoundMatch>
    {
        IQueryable<LostFoundMatch> GetByLostReportId(int lostReportId);
        IQueryable<LostFoundMatch> GetByFoundReportId(int foundReportId);
        bool MatchExists(int lostReportId, int foundReportId);
        IQueryable<int> GetFoundReportIdsForLostReport(int lostReportId);
        IQueryable<int> GetLostReportIdsForFoundReport(int foundReportId);
    }



}
