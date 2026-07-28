using LostFoundPetReporter.CoreDb.repos;
using LostFoundPetReporter.CoreDb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.CoreDb.ReposInterfaces
{
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
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Delete(T entity, bool persist = true);
        int DeleteRange(IEnumerable<T> entities, bool persist = true);
        int SaveChanges();
    }

    public interface IUserRepo : IBaseRepo<User>
    {
        IEnumerable<User> GetAllBy(int id);
        string GetUserName(int id);
        User GetByEmail(string email);
    }

    public interface IFoundReportRepo : IBaseRepo<FoundReport>
    {
        IEnumerable<FoundReport> GetByUserId(int userId);
        IEnumerable<FoundReport> GetReportsWithFiles();
        IEnumerable<FoundReport> GetByPetType(string petType);
    }

    public interface ILostReportRepo : IBaseRepo<LostReport>
    {
        IEnumerable<LostReport> GetByUserId(int userId);
        IEnumerable<LostReport> GetReportsWithFiles();
        IEnumerable<LostReport> GetByPetType(string petType);
    }

    public interface ILostFoundMatchRepo : IBaseRepo<LostFoundMatch>
    {
        IEnumerable<LostFoundMatch> GetMatchesByLostReportId(int lostReportId);
        IEnumerable<LostFoundMatch> GetMatchesByFoundReportId(int foundReportId);
    }

    public interface IFoundReportExtFileRepo : IBaseRepo<FoundReportExtFile>
    {
        IEnumerable<FoundReportExtFile> GetFilesByFoundReportId(int foundReportId);
    }

    public interface ILostReportExtFileRepo : IBaseRepo<LostReportExtFile>
    {
        IEnumerable<LostReportExtFile> GetFilesByLostReportId(int lostReportId);
    }
}
