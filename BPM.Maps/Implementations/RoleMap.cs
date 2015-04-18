using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Maps.Interfaces; 
using BPM.Repositories.DataContext;
using BPM.Services.Interfaces;
using BPM.ViewModels;

namespace BPM.Maps.Implementations
{
    public class RoleMap: IRoleMap
    {
         private IRolService roleService;
       
        public RoleMap() { }
        public RoleMap(IRolService roleService)
        {
            this.roleService = roleService;
        
        }

        public List<RoleViewModel> GetAllActiveRoles()
        {

            List<SisRol> models = roleService.GetActiveRoles();
            var viewModels = new List<RoleViewModel>();
            foreach (SisRol model in models)
            {
                viewModels.Add(ModelToViewModel(model));
            }
            return viewModels;
        }

        public RoleViewModel GetRoleById(int id)
        {
            SisRol model = roleService.GetRolById(id);
            RoleViewModel viewModel = ModelToViewModel(model);

            return viewModel;
        }

        public List<Permissions> GetRolePermission(int id)
        {
            var viewModels = roleService.GetRolePermission(id);
        
            return viewModels;
            
        }

        public RoleViewModel GetRoleByName(string rolename)
        {
            RoleViewModel viewModel = null;
            var model = roleService.GetRolByName(rolename);
            if(model != null)
             viewModel = ModelToViewModel(model);
            return viewModel;
        }

        public int RoleInsert(RoleViewModel rol)
        {
            var role = ViewModelToModel(rol);

            return roleService.RoleInsert(role);
        }

        public int RoleUpdate(RoleViewModel rol)
        {
            var role = ViewModelToModel(rol);

            return roleService.RoleUpdate(role);
        }

        public bool RoleExists(int id)
        {
            return roleService.RoleExists(id);
        }

        public bool RoleDelete(int id)
        {
            return roleService.RoleDelete(id);
        }

        public  RoleViewModel ModelToViewModel(SisRol model)
        {
            var viewModel = new RoleViewModel
            {
                Id = model.rolId,
                Nombre = model.Nombre,
                Activo = model.Activo,
                FechaAlta = model.FechaAlta
            };
            return viewModel;
        }

      
        private static SisRol ViewModelToModel(RoleViewModel viewModel)
        {
            var model = new SisRol
            {
                rolId = viewModel.Id,
                Nombre = viewModel.Nombre,
                Activo = viewModel.Activo,
                FechaAlta = viewModel.FechaAlta
            };
            return model;
        }
    }
}
