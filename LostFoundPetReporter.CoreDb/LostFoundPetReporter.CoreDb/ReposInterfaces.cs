using LostFoundPetReporter.CoreDb.repos;
using LostFoundPetReporter.CoreDb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.CoreDb.ReposInterfaces
{
    public interface IRepositoryFactory
    {
        TRepo CreateRepository<TRepo>() where TRepo : class;
    }

    public interface IBaseViewRepo<T> : IDisposable where T : class, new()
    {
        IEnumerable<T> ExcuteSqlString(string sql);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllIgnoreQueryFillters();

        // in the book they added the EF implementation here but i want to try to decouple , lets see if this will result in an error
        //PetReporterContext Context { get; }
        
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
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Delete(T entity, bool persist = true);
        int DeleteRange(IEnumerable<T> entities, bool persist = true);
        int SaveChanges();
    }

    public interface IUserRepo : IBaseRepo<User>
    {
        IEnumerable<User> GetAllBy(int id);
    }

    public interface IFoundReportRepo : IBaseRepo<FoundReport>
    {
        IEnumerable<FoundReport> GetByUserId(int userId);

    }

    public interface ILostReportRepo : IBaseRepo<LostReport>
    {
        IEnumerable<LostReport> GetByUserId(int userId);
    }

    public interface ILostFoundMatchRepo : IBaseRepo<LostFoundMatch>
    {
        IEnumerable<LostFoundMatch> GetByLostReportId(int lostReportId);
        IEnumerable<LostFoundMatch> GetByFoundReportId(int foundReportId);
        bool MatchExists(int lostReportId, int foundReportId);
    }



}
