using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;
using Microsoft.AspNet.Identity;
//using WebApi.DataAcces;


namespace BPM.Repositories.Interfaces
{
    public interface IAuthRepository : IGenericRepository<SisUsuario>
    {
       /* Task<IdentityResult> RegisterUser(SisUsuario userModel);

         Task<SisUsuario> FindUser(string userName, string password);*/
    }
}
