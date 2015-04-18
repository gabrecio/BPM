using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;
using Microsoft.AspNet.Identity;
using BPM.Repositories.Interfaces;

namespace BPM.Repositories.Implementations
{
    public class AuthRepository : BaseEFRepository<SisUsuario, int>, IAuthRepository
    {
        private readonly FrameworkEntities _dbContext;

        public AuthRepository(FrameworkEntities dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }
        /*

        public async Task<IdentityResult> RegisterUser(SisUsuario userModel)
        {

          
            var h = new Hasher { SaltSize = 16 };
          

            SisUsuario user = new SisUsuario
            {
                Mail = userModel.Mail,
                Password= h.Encrypt(userModel.Password) 
            };
            //var result = await _userManager.CreateAsync(user, userModel.Password);
            Insert(user);
            return new IdentityResult();
           
            //return 1;
        }


        public async Task<SisUsuario> FindUser(string userName, string password)
        {
            try
            {
                var h = new Hasher { SaltSize = 16 };
                var usuario = (from u in db.SisUsuarios
                               where u.Mail == userName 
                               select u).FirstOrDefault();
                if (usuario != null)
                    if (h.CompareStringToHash(password, usuario.Password))
                        return usuario;
                    else
                        return null;
                else
                    return null;
               
            }
            catch(Exception e)
            {

               var ex = e;
               return new SisUsuario();
            }
        }
        */
       /* public async Task<Usuario> FindUser(string userName, string password)
        {
            IdentityUser user = await db.FindAsync(userName, password);

            return user;
        }*/

    
    }
}