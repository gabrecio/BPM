using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.Common;
using BPM.Repositories.DataContext;
using BPM.Repositories.Interfaces;

namespace BPM.Repositories.Implementations
{
    public class ListaPermisoRepository : BaseEFRepository<SisListaPermiso, int>, IListaPermisoRepository
    {
          private readonly FrameworkEntities _dbContext;

          public ListaPermisoRepository(FrameworkEntities dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
