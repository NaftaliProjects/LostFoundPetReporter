using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using LostFoundPetReporter.Services.DataServices.Dal;
using LostFoundPetReporter.Services.DataServices.Interfaces;


namespace LostFoundPetReporter.Services.DataServices.API.Services.DataServices.Api
{
    public abstract class ApiDataServiceBase<TEntity> : IDataServiceBase<TEntity>
        where TEntity : BaseModel, new()
    {
        protected ApiDataServiceBase()
        {

        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => throw new NotImplementedException();
        public async Task<TEntity> FindAsync(int id) => throw new NotImplementedException();
        public async Task<TEntity> UpdateAsync(TEntity entity, bool persist = true)
        {
            throw new NotImplementedException();
        }
        public Task DeleteAsync(TEntity entity, bool persist = true)
            => throw new NotImplementedException();
        public async Task<TEntity> AddAsync(TEntity entity, bool persist = true)
        {
            throw new NotImplementedException();
        }
        public void ResetChangeTracker() { }
    }



    public class UserApiDataService : ApiDataServiceBase<User>, IUserDataService
    {
        public UserApiDataService() : base()
        {
        }

        public async Task<IEnumerable<User>> GetAllByUserIdAsync(int? UserId)
            => throw new NotImplementedException();
    }

    public class LostReportApiDataService : ApiDataServiceBase<LostReport>, ILostReportDataService
    {
        public LostReportApiDataService() : base()
        {
        }

        public async Task<IEnumerable<LostReport>> GetAllByUserIdAsync(int? UserId)
            => throw new NotImplementedException();
    }

    public class FoundReportApiDataService : ApiDataServiceBase<FoundReport>, IFoundReportDataService
    {
        public FoundReportApiDataService() : base()
        {
        }

        public async Task<IEnumerable<FoundReport>> GetAllByUserIdAsync(int? UserId)
            => throw new NotImplementedException();
    }
}
