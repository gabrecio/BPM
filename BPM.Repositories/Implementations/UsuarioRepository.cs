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
    public class UsuarioRepository : BaseEFRepository<SisUsuario, int>, IUsuarioRepository 
    {
       
        private readonly FrameworkEntities _dbContext;

        public UsuarioRepository(FrameworkEntities dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

   /*     public UsuarioRepository()
        {
            _dbContext =  new FrameworkEntities();
        }
     */
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

            user.SisRols.Add((from r in _dbContext.SisRols where r.rolId.Equals(rolId) select r).SingleOrDefault());
            _dbContext.SisUsuarios.Add(user);
            _dbContext.SaveChanges();
            return user.usuarioId;
        }

        public int UserUpdate(SisUsuario user)
        {

            var currentUser = (from u in _dbContext.SisUsuarios where u.usuarioId.Equals(user.usuarioId) select u).SingleOrDefault();
                    var oldRolId= currentUser.SisRols.Select(x => x.rolId).FirstOrDefault();
                    var newRolId = user.SisRols.Select(x => x.rolId).FirstOrDefault();
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
                        currentUser.SisRols.Add((from r in _dbContext.SisRols where r.rolId.Equals(newRolId) select r).SingleOrDefault());
                    }

                    _dbContext.SisUsuarios.Attach(currentUser);
                    _dbContext.Entry(currentUser).State = EntityState.Modified;
                    _dbContext.SaveChanges();
         
            return 1;
        }

        public IEnumerable<string> GetUserPermission(int userId) {
            var permission = new List<string>();
           
                var user = (from u in _dbContext.SisUsuarios                          
                        where u.usuarioId == userId                       
                        select u).SingleOrDefault();
                 permission = (from l in user.SisRols.FirstOrDefault().SisListaPermisoes
                               join menu in _dbContext.SisMenus on l.menuId equals menu.menuId
                               join op in _dbContext.SisOperaciones on l.opId equals op.opId
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