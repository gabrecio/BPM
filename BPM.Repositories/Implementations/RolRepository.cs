using System.Collections.Generic;
using System.Linq;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;
using BPM.Repositories.Interfaces;
using BPM.ViewModels;
using System.Data.Entity;

namespace BPM.Repositories.Implementations
{
    public class RolRepository : BaseEFRepository<SisRol, int>, IRolRepository
    {
        private readonly FrameworkEntities _dbContext;

        public RolRepository(FrameworkEntities dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public SisRol GetRolByName(string rolname)
        {
          
                return (from u in _dbContext.SisRols
                    where u.Nombre == rolname
                    select u).SingleOrDefault();
           
        }

        public bool RolExists(int id)
        {
            return _dbContext.SisRols.Count(e => e.rolId == id) > 0;
        }

        public bool RolHasUsers(int id)
        {
            var rol = _dbContext.SisRols.FirstOrDefault(r => r.rolId.Equals(id));
            return rol != null && rol.SisUsuarios.Any();
        }

        public List<Permissions> GetRolPermission(int rolId)
        {
            var rolPermissions = new List<Permissions>();
          
                var rolPermission = new List<int>();
                if (rolId != 0)
                {
                    rolPermission = (from rol in _dbContext.SisRols.FirstOrDefault(i => i.rolId == rolId).SisListaPermisoes select rol.lipId).ToList();
                }

                foreach (var menu in _dbContext.SisMenus)
                {
                    var lista = new List<Permission>();

                    foreach (var ope in (from lp in _dbContext.SisMenus.FirstOrDefault(i => i.menuId == menu.menuId).SisListaPermisoes
                                         join op in _dbContext.SisOperaciones on lp.opId equals op.opId
                                         select new { lpId = lp.lipId, nombre = op.Nombre, imagen = op.Imagen }).ToList())
                    {
                        var np = new Permission()
                        {
                            Operacion = ope.nombre,
                            Activo = rolPermission.Contains(ope.lpId),
                            Imagen = ope.imagen,
                            ListaPermisoId = ope.lpId
                        };
                        lista.Add(np);

                    }

                    var perm = new Permissions()
                    {
                        Menu = menu.Titulo,
                        Imagen = menu.Imagen,
                        Operaciones = lista
                    };
                    rolPermissions.Add(perm);
                }
            
            return rolPermissions;
        }

        public List<int> GetListaPermisoByRol(int id)
        {
            var firstOrDefault = _dbContext.SisRols.FirstOrDefault(z => z.rolId.Equals(id));
            if (firstOrDefault != null)
                return firstOrDefault.SisListaPermisoes.Select(x => x.lipId).ToList();
            return new List<int>();
        }

        public int RolInsert(SisRol rol)
        {
            
                _dbContext.SisRols.Add(rol);
                _dbContext.SaveChanges();
                return rol.rolId;
        
        }

        

        public int RolUpdate(int id, SisRol rol)
        {
          
            
                var currentRol =  this.GetById(rol.rolId);
                currentRol.Activo= rol.Activo;
                currentRol.Nombre = rol.Nombre; 

                foreach (var item in rol.SisListaPermisoes.Where(x => !currentRol.SisListaPermisoes.Select(l=>l.lipId).Contains(x.lipId)).ToList())
                {
                    //inserta nuevos
                    currentRol.SisListaPermisoes.Add(item);
                }

                foreach (var item in currentRol.SisListaPermisoes.Where(x=> !rol.SisListaPermisoes.Select(l=>l.lipId).Contains(x.lipId)).ToList())
                {
                    //elimina items 
                    currentRol.SisListaPermisoes.Remove(item);
                }


                 _dbContext.SisRols.Attach(currentRol);
                 _dbContext.Entry(currentRol).State = EntityState.Modified;
                 _dbContext.SaveChanges();
             
            
            return 1;
        }

    }
}