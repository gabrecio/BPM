using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;
using BPM.Repositories.Common;
using BPM.Repositories.DataContext;
using BPM.Repositories.Interfaces;
using BPM.Repositories;
using System.Data.Entity;
using BPM.ViewModels;

namespace BPM.Repositories.Implementations
{
    public class UsuarioRepository : GenericRepository<SisUsuario>, IUsuarioRepository 
    {
       
        private readonly FrameworkEntities _dbContext;

        public UsuarioRepository(FrameworkEntities dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public UsuarioRepository()
        {
            _dbContext =  new FrameworkEntities();
        }
     
        public SisUsuario GetUserByName(string username)
        {
            return (from u in _dbContext.SisUsuarios
                    where u.Mail == username
                    select u).SingleOrDefault();            
        }

        public bool UserExists(int id)
        {
            return _dbContext.SisUsuarios.Count(e => e.usuarioId == id) > 0;
        }

        public int UserInsert(SisUsuario user, int rolId)
        {
           /* using (var db = new BPMContext()) 
            {
                user .SisRols.Add((from r in db.SisRols where r.rolId.Equals(rolId) select r).SingleOrDefault());
                db.SisUsuarios.Add(user);
                db.SaveChanges();                
            }*/
            return user.usuarioId;
        }

        public int UserUpdate(SisUsuario user)
        {
           /* using (var db = new BPMContext())
            {         
                    var currentUser = (from u in db.SisUsuarios where u.usuarioId.Equals(user.Id) select u).SingleOrDefault();
                    var oldRolId= currentUser.SisRols.Select(x => x.rolId).FirstOrDefault();
                    var newRolId = user.roles.Select(x => x.Id).FirstOrDefault();
                    currentUser.Nombre = user.Nombre;
                    currentUser.Apellido = user.Apellido;
                    currentUser.Activo = true;
                    if (!String.IsNullOrEmpty(user.Password))
                    {
                        var h = new Hasher { SaltSize = 16 };
                        currentUser.Password = h.Encrypt(user.Password);
                    }
                    if (oldRolId != newRolId)
                    {
                        currentUser.SisRols.Remove(currentUser.SisRols.Where(x => x.rolId.Equals(oldRolId)).SingleOrDefault());
                        currentUser.SisRols.Add((from r in db.SisRols where r.rolId.Equals(newRolId) select r).SingleOrDefault());
                    }
                

                db.SisUsuarios.Attach(currentUser);
                db.Entry(currentUser).State = EntityState.Modified;             
                db.SaveChanges();
            }*/
            return 1;
        }

        public IEnumerable<string> GetUserPermission(int userId) {
            var permission = new List<string>();
           
                var user = (from u in _dbContext.SisUsuarios                          
                        where u.usuarioId == userId                       
                        select u).SingleOrDefault();
                 permission = (from l in user.SisRols.FirstOrDefault().SisListaPermisoes
                            join menu in db.SisMenus on l.menuId equals menu.menuId
                            join op in db.SisOperaciones on l.opId equals op.opId
                            select  menu.Titulo + "." + op.Nombre ).ToList();
                
          
            return permission ;
         }


        public SisUsuario FindUser(string userName, string password)
        {
            try
            {
                var h = new Hasher { SaltSize = 16 };
                var usuario = (from u in _dbContext.SisUsuarios
                               where u.Mail == userName
                               select u).FirstOrDefault();
                if (usuario != null)
                    if (h.CompareStringToHash(password, usuario.Password))
                        return usuario;
                    else
                        return null;
                return null;

            }
            catch (Exception e)
            {

                var ex = e;
                return new SisUsuario();
            }
        }

    }
}