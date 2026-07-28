using System;
using System.Collections.Generic;
using System.Text;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace LostFoundPetReporter.CoreDb.repos
{
    

    public abstract class BaseViewRepo<T> : IBaseViewRepo<T> where T : class, new()
    {
        private readonly bool _disoposeContext;
        public PetReporterContext Context { get; }
        public DbSet<T> Table { get; }

        protected BaseViewRepo(PetReporterContext context)
        {
            Context = context;
            Table = Context.Set<T>();
            _disoposeContext = false;
        }

        protected BaseViewRepo(DbContextOptions<PetReporterContext> options) : this(new PetReporterContext(options))
        {
            _disoposeContext = true;
        }
        public virtual void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDissposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_isDissposed) 
            {
                return;
            }

            if (disposing)
            {
                if (_disoposeContext)
                {
                    Context.Dispose();
                }
            }
            _isDissposed = true;
        }
        ~BaseViewRepo()
        {
            Dispose(false); 
        }

        public virtual IEnumerable<T> ExcuteSqlString(string sql) 
            => Table.FromSqlRaw(sql);
        public virtual IEnumerable<T> GetAll() 
            => Table.AsQueryable();
        public virtual IEnumerable<T> GetAllIgnoreQueryFillters() 
            => Table.AsQueryable().IgnoreQueryFilters();

    }




    public abstract class BaseRepo<T> : BaseViewRepo<T>, IBaseRepo<T> where T : BaseModel, new()
    {
        protected BaseRepo(PetReporterContext context) : base(context) { }
        protected BaseRepo(DbContextOptions<PetReporterContext> options) : this(new PetReporterContext(options)) 
        {
        }

        public int SaveChanges()
        {
            try 
            {
                return Context.SaveChanges();  
            }
            catch (Exception ex) 
            { 
                throw new Exception("An error occurred updating the database", ex); 
            }
        }

        public virtual T Find(int? id) => Table.Find(id);
        public virtual T FindAsNoTracking(int id)
            => Table.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.Id == id);
        public virtual T FindIgnoreQueryFilters(int id) => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
        public virtual void ExecuteQueryFilters(int id) => Table.FirstOrDefault(x => x.Id == id);
        public virtual void ExecutePatameterizedQuery(string sql, object[] sqlParametersObjects)
            => Context.Database.ExecuteSqlRaw(sql, sqlParametersObjects);
        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }
        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }
        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? SaveChanges() : 0;
        }
        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0; 
        }
        public virtual int Delete(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }
        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }
        public int Delete(int id, byte[] timeStamp, bool persist = true)
        {
            var entity = new T { Id = id, TimeStamp = timeStamp };
            Context.Entry(entity).State = EntityState.Deleted;
            return persist ? SaveChanges() : 0;
        }
    }

    //Entity-Specific Repositories Imp
    public class UserRepo : BaseRepo<User>, IUserRepo
    {
        public UserRepo(PetReporterContext context) : base(context)
        {
        }

        internal UserRepo(DbContextOptions<PetReporterContext> options) : base(options)
        {
        }

        public override IEnumerable<User> GetAll()
            => Table.OrderBy(u => u.Name); 


    }

}
