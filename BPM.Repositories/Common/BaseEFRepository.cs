using BPM.Repositories.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BPM.Repositories.Common
{
    public class BaseEFRepository<TEntity, TKey>
        where TEntity : class
    {
        internal FrameworkEntities dbContext;
        internal DbSet<TEntity> dbSet;

        public BaseEFRepository(FrameworkEntities context)
        {
            if (context == null) throw new ArgumentNullException("dbContext");
            this.dbContext = context;
            this.dbSet = context.Set<TEntity>();
        }

        public FrameworkEntities DbContext
        {
            get { return dbContext; }
        }

        /// <summary>
        /// Get method
        /// </summary>
        /// <param name="filter">uses lambda expressions to allow the calling code to specify a filter condition. Eg: if the repository is instantiated for the User entity type, the code in the calling method might specify u => u.UserName == "leoh" for the filter parameter.</param>
        /// <param name="orderBy">uses lambda expressions to allow the calling code to specify a column to order the results by. Eg if the repository is instantiated for the User entity type, the code in the calling method might specify u => q.OrderBy(u => u.UserName) for the orderBy parameter.</param>
        /// <param name="includeProperties">a string parameter that lets the caller provide a comma-delimited list of navigation properties for eager loading</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetById(TKey id)
        {
            return dbSet.Find(id);
        }


        public virtual void Create(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            dbSet.Add(entity);
        }

        public virtual void Delete(TKey id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            dbSet.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
