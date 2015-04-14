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

        public List<string> GetRolePermission(int id)
        {
            return new List<string>();
        }

        public RoleViewModel GetRoleByName(string rolename)
        {
            SisRol model = roleService.GetRolByName(rolename);
            RoleViewModel viewModel = ModelToViewModel(model);
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

        private RoleViewModel ModelToViewModel(SisRol model)
        {
            var viewModel = new RoleViewModel();
            viewModel.Id = model.rolId;
            viewModel.Nombre = model.Nombre;
            return viewModel;
        }

        private SisRol ViewModelToModel(RoleViewModel viewModel)
        {
            var model = new SisRol();
            model.rolId = viewModel.Id;
            model.Nombre = viewModel.Nombre;
            return model;
        }
    }
}
