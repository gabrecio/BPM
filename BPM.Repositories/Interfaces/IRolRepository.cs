using System.Collections.Generic;
using BPM.Repositories.Common;
using BPM.Repositories.DataContext;

namespace BPM.Repositories.Interfaces
{
    public interface IRolRepository : IGenericRepository<SisRol>
    {
        SisRol GetRolByName(string rolname);
        bool RolExists(int id);
        List<SisListaPermiso> GetRolPermission(int rolId);
        int RolInsert(SisRol rol);
        int RolUpdate(int id, SisRol rol);
        bool RolHasUsers(int id);
    }
}
