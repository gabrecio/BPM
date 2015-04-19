using System;
using System.Collections.Generic;

using BPM.Repositories.DataContext;
using BPM.ViewModels;

namespace BPM.Services.Interfaces
{
    public interface IRolService
    {
        List<SisRol> GetActiveRoles();
        SisRol GetRolById(int id);
        SisRol GetRolByName(string roleName);
        int RoleInsert(SisRol rol);
        int RoleUpdate(SisRol rol);
        bool RoleExists(int id);
        bool RoleDelete(int id);
        List<Permissions> GetRolePermission(int id);
        List<int> GetListaPermisoByRol(int id);
    }
}
