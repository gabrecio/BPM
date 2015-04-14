using BPM.Repositories.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Repositories.Common
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        FrameworkEntities DbContext { get; }

        IEnumerable<TEntity> Get(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "");

        TEntity GetById(TKey id);

        void Create(TEntity entity);

        void Delete(TKey id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entity);
    }
}
