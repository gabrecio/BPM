using System;
using System.Collections.Generic;
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

        public UserMap()
        {
            this.userService = new UserService(); 
        }
        public UserMap(IUserService userService)
        {
            this.userService = userService;
        }

        public List<UserViewModel> GetAllActiveUsers()
        {
            List<SisUsuario> models = userService.GetActiveUsers();
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
            UserViewModel viewModel = ModelToViewModel(model);
            return viewModel;
        }

        private UserViewModel ModelToViewModel(SisUsuario model)
        {
            var viewModel = new UserViewModel();
            viewModel.Id = model.usuarioId;
            viewModel.Nombre = model.Nombre;
            return viewModel;
        }

        private SisUsuario ViewModelToModel(UserViewModel viewModel)
        {
            var model = new SisUsuario();
            model.usuarioId = viewModel.Id;
            model.Nombre = viewModel.Nombre ;
            return model;
        }

        public int UserInsert(UserViewModel user, int rolId)
        {
            var usuario = ViewModelToModel(user);

            return userService.UserInsert(usuario, rolId);
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
