
using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.CoreDb.ReposInterfaces
{
    public interface IRepositoryFactory
    {
        TRepo CreateRepository<TRepo>() where TRepo : class;
    }

    public interface IBaseRepo<T> where T : BaseModel
    {
        PetReporterContext Context { get; }

        // Read operations
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllIgnoreQueryFillters();

        T Find(int? id);
        T FindAsNoTracking(int id);
        T FindIgnoreQueryFilters(int id);

        // Create operations
        int Add(T entity, bool persist = true);
        int AddRange(IEnumerable<T> entities, bool persist = true);

        // Update operations
        int Update(T entity, bool persist = true);
        int Update(T entity, T updatedEntity, bool persist = true);
        int UpdateRange(IEnumerable<T> entities, bool persist = true);

        // Delete operations
        int Delete(T entity, bool persist = true);
        int DeleteRange(IEnumerable<T> entities, bool persist = true);

        // Persistence
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
        User? GetByEmail(string email);
    }

    public interface IReportRepo : IBaseRepo<Report>
    {
        IEnumerable<Report> GetAllByUserId(int userId);

        IEnumerable<Report> GetReportsInDateRange(
            DateTime fromDate,
            DateTime toDate);
    }

    public interface IFoundReportRepo : IReportRepo
    {

    }

    public interface ILostReportRepo : IReportRepo
    {

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
