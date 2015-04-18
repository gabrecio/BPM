using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.ViewModels;

namespace BPM.Maps.Interfaces
{
    public interface IUserMap
    {
        List<UserViewModel> GetAllActiveUsers(string query);
        UserViewModel GetUserById(int id);
        //List<string> GetUserPermission(int id);
        UserViewModel GetUserByName(string username);
        int UserInsert(UserViewModel user, int rolId);
        int UserUpdate(UserViewModel user);
        bool UserExists(int id);
        bool UserDelete(int id);
        SisUsuario FindUser(string userName, string password);
        IEnumerable<string> GetUserPermission(int userId);
    }
}
