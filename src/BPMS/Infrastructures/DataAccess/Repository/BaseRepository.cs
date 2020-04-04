using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using BPMS.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Infrastructures.DataAccess.Repository
{
    public class BaseRepository<TEntity> : BaseQueryService<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public BaseRepository(BpmsDbContext context) : base(context)
        {
        }

        public void Update(TEntity retailer)
        {
            dataSet.Update(retailer);
        }

        public virtual async Task Add(TEntity entity)
        {
            await dataSet.AddAsync(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            dataSet.Remove(entity);
        }
    }

    public class BaseQueryService<TEntity> : IQueryService<TEntity> where TEntity : class
    {
        protected readonly BpmsDbContext context;
        protected readonly DbSet<TEntity> dataSet;

        public BaseQueryService(BpmsDbContext context)
        {
            this.context = context;
            dataSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await dataSet.ToListAsync();
        }

        public virtual IQueryable<TEntity> Get()
        {
            return dataSet;
        }

        public async Task<IEnumerable<TEntity>> Get(string query, string orderby, /*Expression<Func<TEntity, object>>[] includes,*/int pageNumber = 1, int pageSize = 25)
        {
            // IIncludableQueryable<TEntity>
            return await dataSet.Where("HixCode=@0", "6651651600")/*
                .OrderBy(orderby)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize)*/.ToListAsync();
        }

        public virtual async Task<TEntity> GetById(object id)
        {
            return await dataSet.FindAsync(id);
        }
    }
}