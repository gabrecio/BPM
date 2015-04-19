using System.Collections.Generic;
using BPM.Repositories.Common;
using BPM.Repositories.DataContext;
using BPM.ViewModels;

namespace BPM.Repositories.Interfaces
{
    public interface IRolRepository :   IRepository<SisRol, int>
    {
        SisRol GetRolByName(string rolname);
        bool RolExists(int id);
        List<Permissions> GetRolPermission(int rolId);
        int RolInsert(SisRol rol);
        int RolUpdate(int id, SisRol rol);
        bool RolHasUsers(int id);
        List<int> GetListaPermisoByRol(int id);
    }
}
