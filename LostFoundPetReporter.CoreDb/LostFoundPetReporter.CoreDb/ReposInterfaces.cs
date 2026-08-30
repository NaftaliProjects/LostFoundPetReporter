
using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.CoreDb.ReposInterfaces
{
    public interface IRepositoryFactory
    {
        TRepo CreateRepository<TRepo>() where TRepo : class;
    }

    public interface IBaseViewRepo<T> : IDisposable where T : class, new()
    {
        PetReporterContext Context { get; }
        IEnumerable<T> ExcuteSqlString(string sql);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllIgnoreQueryFillters();

    }



    public interface IBaseRepo<T> : IBaseViewRepo<T> where T : BaseModel, new()
    {
        T Find(int? id);
        T FindAsNoTracking(int id);
        T FindIgnoreQueryFilters(int id);
        void ExecuteQueryFilters(int id);
        int Add(T entity, bool persist = true);
        int AddRange(IEnumerable<T> entities, bool persist = true);
        int Update(T entity, bool persist = true);
        int Update(T entity,T updatedEntity, bool persist = true);
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Delete(T entity, bool persist = true);
        int DeleteRange(IEnumerable<T> entities, bool persist = true);
        int SaveChanges();
    }

    public interface IUserRepo : IBaseRepo<User>
    {
        IEnumerable<User> GetAllByUserId(int id);

        public User? GetByEmail(string email);
    }

    public interface IFoundReportRepo : IBaseRepo<FoundReport>
    {
        IEnumerable<FoundReport> GetAllByUserId(int userId);
        IEnumerable<FoundReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate);

    }

    public interface ILostReportRepo : IBaseRepo<LostReport>
    {
        IEnumerable<LostReport> GetAllByUserId(int userId);
        IEnumerable<LostReport> GetReportsInDateRange(DateTime fromDate, DateTime toDate);
    }

    public interface ILostFoundMatchRepo : IBaseRepo<LostFoundMatch>
    {
        IEnumerable<LostFoundMatch> GetByLostReportId(int lostReportId);
        IEnumerable<LostFoundMatch> GetByFoundReportId(int foundReportId);
        bool MatchExists(int lostReportId, int foundReportId);
        IEnumerable<int> GetFoundReportIdsForLostReport(int lostReportId);
        IEnumerable<int> GetLostReportIdsForFoundReport(int foundReportId);
    }



}
