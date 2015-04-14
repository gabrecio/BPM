using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.Common;

namespace BPM.Services
{
    public class BaseService<TEntity, TKey, TRepository>
        where TEntity : class
        where TRepository : BaseEFRepository<TEntity, TKey>
    {
        internal TRepository repository;

        public BaseService(TRepository repository)
        {
            this.repository = repository;
        }

        public virtual IList<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            return repository.Get(filter, orderBy, includeProperties).ToList();
        }

        public virtual TEntity GetById(TKey id)
        {
            return repository.GetById(id);
        }


        public virtual void Create(TEntity entity)
        {
            repository.Create(entity);
        }

        public virtual void Delete(TKey id)
        {
            repository.Delete(id);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            repository.Delete(entityToDelete);
        }

        public virtual void Update(TEntity entity)
        {
            repository.Update(entity);
        }
    }
}
