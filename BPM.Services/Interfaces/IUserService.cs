using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.ViewModels;

namespace BPM.Services.Interfaces
{
    public interface IUserService
    {
        List<SisUsuario> GetActiveUsers();
        SisUsuario GetUserById(int id);
        SisUsuario GetUserByName(string userName);
        int UserInsert(SisUsuario user, int rolId);
        int UserUpdate(SisUsuario user);
        bool UserExists(int id);
        bool UserDelete(int id);
        SisUsuario FindUser(string userName, string password);
        IEnumerable<string> GetUserPermission(int userId);
    }
}
