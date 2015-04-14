using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.ViewModels;

namespace BPM.Maps.Interfaces
{
    public interface IRoleMap
    {
        List<RoleViewModel> GetAllActiveRoles();
        RoleViewModel GetRoleById(int id);
        List<string> GetRolePermission(int id);
        RoleViewModel GetRoleByName(string rolename);
        int RoleInsert(RoleViewModel role);
        int RoleUpdate(RoleViewModel role);
        bool RoleExists(int id);
        bool RoleDelete(int id);
    }
}
