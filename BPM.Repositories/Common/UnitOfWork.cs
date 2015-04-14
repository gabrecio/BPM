using BPM.Repositories.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Repositories.Common
{
    /// <summary>
    /// Use the UOW pattern to make sure that when you use multiple repositories, they share a single database context.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FrameworkEntities  context;

        public UnitOfWork(FrameworkEntities context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
