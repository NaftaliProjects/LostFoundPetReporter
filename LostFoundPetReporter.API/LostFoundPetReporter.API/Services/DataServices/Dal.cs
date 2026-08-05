using LostFoundPetReporter.API.Services.DataServices.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Services.DataServices
{
    public abstract class DalDataServiceUser<TEntity> : IDataServiceBase<TEntity>
        where TEntity : BaseModel, new()
    {
        protected readonly IBaseRepo<TEntity> MainRepo;

        protected DalDataServiceUser(IBaseRepo<TEntity> mainRepo)
        {
            this.MainRepo = mainRepo;
        }

        //public Task<IEnumerable<TEntity>> GetAllAsync()
          //  => 

        public Task<TEntity> FindAsync(int id);
        public Task<TEntity> UpdateAsync(TEntity entity, bool persist = true);
        public Task DeleteAsync(TEntity entity, bool persist = true);
        public void ResetChangeTracker() { }

        public Task<IEnumerable<User>> GetAllByCarIdAsync();
    }
}
