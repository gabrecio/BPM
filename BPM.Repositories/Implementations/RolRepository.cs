using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;
using BPM.Repositories.Interfaces;

namespace BPM.Repositories.Implementations
{
    public class RolRepository : GenericRepository<SisRol>, IRolRepository
    {
        private readonly FrameworkEntities _dbContext;

        public RolRepository(FrameworkEntities dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public SisRol GetRolByName(string rolname)
        {
            using (var db = new FrameworkEntities()) 
            {
                return (from u in db.SisRols
                    where u.Nombre == rolname
                    select u).SingleOrDefault();
            }
        }

        public bool RolExists(int id)
        {
            return _dbContext.SisRols.Count(e => e.rolId == id) > 0;
        }

        public bool RolHasUsers(int id)
        {
            var rol = _dbContext.SisRols.FirstOrDefault(r => r.rolId.Equals(id));
            return true; //rol != null && rol .SisUsuarios.Any();
        }

        public List<SisListaPermiso> GetRolPermission(int rolId)
        {
            var rolPermissions = new List<SisListaPermiso>();
           /* using (var db = new BPMContext())
            {
                var rolPermission = new List<int>();
                if (rolId != 0) {
                   rolPermission=  (from rol in db.SisRoles.FirstOrDefault(i => i.Id == rolId).SisListaPermisoes select rol.lipId).ToList();
                }

                foreach (var menu in db.SisMenus)
                {
                    var lista = new List<SisRolPermiso>();

                    foreach (var ope in (from lp in db.SisMenus.FirstOrDefault(i => i.menuId == menu.menuId).SisListaPermisoes
                                         join op in db.SisOperaciones on lp.opId equals op.opId
                                         select new{lpId= lp.lipId, nombre=op.Nombre, imagen=op.Imagen}).ToList())
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

                    var perm = new SisRolPermiso()
                    {
                        Menu = menu.Titulo,
                        Imagen = menu.Imagen,
                        Operaciones = lista
                    };
                    rolPermissions.Add(perm);
                }
            }*/
            return rolPermissions;
        }

        public int RolInsert(SisRol rol)
        {

           /* using (var entities = new BPMContext()) 
            {
                var newRole = SisRol.For(rol);
              
                foreach (var item in rol.Permissions.Where(x=>x.Activo).ToList())
                {
                    newRole.SisListaPermisoes.Add((from r in entities.SisListaPermisoes where r.lipId.Equals(item.ListaPermisoId) select r).FirstOrDefault()); 
                }
                entities.SisRols.Add(newRole);
                entities.SaveChanges();
                return newRole.rolId;
            }*/
            return 0;
        }

        public int RolUpdate(int id, SisRol rol)
        {

         /*   using (var db = new BPMContext())
            {
                var currentRol = (from u in db.SisRoles where u.Id.Equals(id) select u).SingleOrDefault();
                currentRol.Activo=rol.Activo;
                
                var ls = db.SisRols.Where(z => z.rolId.Equals(id)).FirstOrDefault().SisListaPermisoes.Select(x=>x.lipId).ToList();

                foreach (var item in rol.Permissions.Where(x => x.Activo && !ls.Contains(x.ListaPermisoId)).ToList())
                {
                    //inserta nuevos
                    currentRol.SisListaPermisoes.Add((from r in db.SisListaPermisoes where r.lipId.Equals(item.ListaPermisoId) select r).FirstOrDefault());
                }

                 foreach (var item in rol.Permissions.Where(x => !x.Activo && ls.Contains(x.ListaPermisoId)).ToList())
                {
                    //elimina items 
                    currentRol.SisListaPermisoes.Remove(currentRol.SisListaPermisoes.FirstOrDefault(r => r.lipId.Equals(item.ListaPermisoId)));
                }


                 db.SisRols.Attach(currentRol);
                 db.Entry(currentRol).State = EntityState.Modified;
                 db.SaveChanges();
             
            }*/
            return 1;
        }

    }
}