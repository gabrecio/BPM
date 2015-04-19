using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.Repositories.Interfaces;
using BPM.Services.Interfaces;
using BPM.ViewModels;

namespace BPM.Services.Implementations
{
    public class RolService: IRolService
    {
        private IRolRepository rolRepository;
      


        public RolService(IRolRepository rolRepository)
        {
            this.rolRepository = rolRepository;
        
        }


        public List<SisRol> GetActiveRoles()
        {
            return (List<SisRol>)rolRepository.Get();
        }

        public SisRol GetRolById(int id)
        {
            return rolRepository.GetById(id);
        }

        public SisRol GetRolByName(string roleName)
        {
            return rolRepository.GetRolByName(roleName);
        }

        public int RoleInsert(SisRol rol)
        {
            return rolRepository.RolInsert(rol);
        }

        public int RoleUpdate(SisRol rol)
        { //quitar parametro id
            return rolRepository.RolUpdate(1,rol);
        }

        public bool RoleExists(int id)
        {
            return rolRepository.RolExists(id);
        }

        public bool RoleDelete(int id)
        {
            var rol = rolRepository.GetById(id);
            rolRepository.Delete(rol);
            return true;
        }

        public List<Permissions> GetRolePermission(int id)
        {
            return rolRepository.GetRolPermission(id);
          
        }

        public List<int> GetListaPermisoByRol(int id)
        {
            return rolRepository.GetListaPermisoByRol(id);
        }
    }
}
