using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPM.Services.Implementations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BPM.Maps.Interfaces;
using BPM.Repositories.DataContext;
using BPM.Services.Interfaces;
using BPM.ViewModels;

namespace BPM.Maps.Implementations
{
    public class UserMap : IUserMap
    {
        private IUserService userService;
        private IRolService roleService;

        public UserMap(IUserService userService, IRolService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }

        public List<UserViewModel> GetAllActiveUsers(string query)
        {
            List<SisUsuario> models = userService.GetActiveUsers(query);
            var viewModels = new List<UserViewModel>();
            foreach (SisUsuario model in models)
            {
                viewModels.Add(ModelToViewModel(model));
            }
            return viewModels;

        }

        public UserViewModel GetUserById(int id)
        {
            SisUsuario model = userService.GetUserById(id);
            UserViewModel viewModel = ModelToViewModel(model);
            
            return viewModel;

        }

        //public List<string> GetUserPermission(int id)
        //{
        //    return new List<string>();
        //}

        public UserViewModel GetUserByName(string username)
        {
            SisUsuario model = userService.GetUserByName(username);
            UserViewModel viewModel = null;
            if(model != null) viewModel = ModelToViewModel(model);
            return viewModel;
        }

        private UserViewModel ModelToViewModel(SisUsuario model)
        {
            var viewModel = new UserViewModel();
            viewModel.Id = model.usuarioId;
            viewModel.Nombre = model.Nombre;
            viewModel.Mail = model.Mail;
            viewModel.Activo = model.Activo;
            viewModel.Apellido = model.Apellido;
            viewModel.FechaAlta = model.FechaAlta;
            var rol = new RoleMap();
            var viewModels = model.SisRols.Select(rolModel => rol.ModelToViewModel(rolModel)).ToList();
            viewModel.roles = viewModels;
            return viewModel;
        }

        private SisUsuario ViewModelToModel(UserViewModel viewModel)
        {
            var model = new SisUsuario();
            model.usuarioId = viewModel.Id;
            model.Nombre = viewModel.Nombre;
            model.Mail = viewModel.Mail;
            model.Activo = viewModel.Activo;
            model.Apellido = viewModel.Apellido;
            model.Password = viewModel.Password;
            model.FechaAlta = (new DateTime(2010, 5, 1, 8, 30, 52).CompareTo(viewModel.FechaAlta) > 0)
                ? DateTime.Now
                : viewModel.FechaAlta;
            model.SisRols = new List<SisRol>();
            if (viewModel.roles != null)
            {
                foreach (RoleViewModel rol in viewModel.roles)
                {
                    model.SisRols.Add( roleService.GetRolById(rol.Id));
                }
            }

            return model;
        }

        public int UserInsert(UserViewModel user)
        {
            var usuario = ViewModelToModel(user);

            return userService.UserInsert(usuario);
        }

        public int UserUpdate(UserViewModel user)
        {
            var usuario = ViewModelToModel(user);

            return userService.UserUpdate(usuario);
        }

        public bool UserExists(int id)
        {
            return userService.UserExists(id);
        }
        public bool UserDelete(int id)
        {
           
            return userService.UserDelete(id);
        }

        public SisUsuario FindUser(string userName, string password)
        {
            return userService.FindUser(userName, password);
        }
        public IEnumerable<string> GetUserPermission(int userId) {
            return  userService.GetUserPermission(userId);
        }

    }
}
