using LostFoundPetReporter.Services.DataServices.Interfaces;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.Services.DataServices.Dal
{
    public abstract class DalDataServiceBase<TEntity> : IDataServiceBase<TEntity>
        where TEntity : BaseModel, new()
    {
        protected readonly IBaseRepo<TEntity> MainRepo;

        protected DalDataServiceBase(IBaseRepo<TEntity> mainRepo)
        {
            this.MainRepo = mainRepo;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => MainRepo.GetAllIgnoreQueryFillters();
        public async Task<TEntity> FindAsync(int id) => MainRepo.Find(id);
        public async Task<TEntity> UpdateAsync(TEntity entity, bool persist = true)
        {
            MainRepo.Update(entity, persist);
            return entity;
        }   
        public async Task DeleteAsync(TEntity entity, bool persist = true)
            => MainRepo.Delete(entity, persist);
        public async Task<TEntity> AddAsync(TEntity entity, bool persist = true)
        {
            MainRepo.Add(entity, persist);
            return entity;
        }
        public void ResetChangeTracker() 
        {
            MainRepo.Context.ChangeTracker.Clear();
        }


    }





    public class UserDalDataService : DalDataServiceBase<User>, IUserDataService
    {
        private readonly IUserRepo _repo;
        public UserDalDataService(IUserRepo repo) : base(repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<User>> GetAllByUserIdAsync(int? UserId)
            => UserId.HasValue
                ? _repo.GetAllBy(UserId.Value)
                : MainRepo.GetAllIgnoreQueryFillters();
       
    }


    public class LostReportDalDataService : DalDataServiceBase<LostReport>, ILostReportDataService
    {
        private readonly ILostReportRepo _repo;

        public LostReportDalDataService(ILostReportRepo repo) : base(repo) { }

        public async Task<IEnumerable<LostReport>> GetAllByUserIdAsync(int? UserId)
            => UserId.HasValue
                ? _repo.GetByUserId(UserId.Value)
                : MainRepo.GetAllIgnoreQueryFillters();
    }


    public class FoundReportDalDataService : DalDataServiceBase<FoundReport>, IFoundReportDataService
    {
        private readonly IFoundReportRepo _repo;

        public FoundReportDalDataService(IFoundReportRepo repo) : base(repo) { }

        public async Task<IEnumerable<FoundReport>> GetAllByUserIdAsync(int? UserId)
            => UserId.HasValue
                ? _repo.GetByUserId(UserId.Value)
                : MainRepo.GetAllIgnoreQueryFillters();
    }
}
