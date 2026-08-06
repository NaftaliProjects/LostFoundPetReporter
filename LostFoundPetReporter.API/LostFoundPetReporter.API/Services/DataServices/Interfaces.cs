using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.Services.DataServices.Interfaces
{
    public interface IDataServiceBase<TEntity> where TEntity : BaseModel
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<TEntity> FindAsync(int id);
        public Task<TEntity> UpdateAsync(TEntity entity, bool persist = true);
        public Task DeleteAsync(TEntity entity, bool persist = true);
        public Task<TEntity> AddAsync(TEntity entity, bool persist = true);
        public void ResetChangeTracker() { }

    }

    public interface IUserDataService : IDataServiceBase<User>
    {
        public Task<IEnumerable<User>> GetAllByUserIdAsync(int? UserId);
    }

    public interface ILostReportDataService : IDataServiceBase<LostReport>
    {
        public Task<IEnumerable<LostReport>> GetAllByUserIdAsync(int? UseId);
    }

    public interface IFoundReportDataService : IDataServiceBase<FoundReport>
    {
        public Task<IEnumerable<FoundReport>> GetAllByUserIdAsync(int? UseId);
    }


}
