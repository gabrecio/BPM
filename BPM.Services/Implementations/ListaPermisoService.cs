using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.Repositories.Interfaces;
using BPM.Services.Interfaces;

namespace BPM.Services.Implementations
{
    public class ListaPermisoService : IListaPermisoService
    {
          private IListaPermisoRepository listaPermisoRepository;

          public ListaPermisoService(IListaPermisoRepository listaPermisoRepository)
        {
            this.listaPermisoRepository = listaPermisoRepository;
        }
          public SisListaPermiso GetListaPermisoById(int id)
          {
              return listaPermisoRepository.GetById(id);
          }
    }
}
