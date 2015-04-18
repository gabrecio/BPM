using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPM.Repositories.DataContext;
using BPM.Repositories.Implementations;
using BPM.Repositories.Interfaces;
using BPM.Services.Interfaces;
using BPM.ViewModels;

namespace BPM.Services.Implementations
{
    public class UserService : IUserService
    {
        private IUsuarioRepository userRepository;

        public UserService(IUsuarioRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public List<SisUsuario> GetActiveUsers(string query)
        {
            if(String.IsNullOrEmpty(query))
                   return (List<SisUsuario>)userRepository.Get();
            else
            {
                return (List<SisUsuario>)userRepository.Get().Where(au => au.Mail.ToLower().Contains(query) || au.Nombre.ToLower().Contains(query) || au.Apellido.ToLower().Contains(query)).ToList();
            }
         
        }

        public SisUsuario GetUserById(int id)
        {
            return userRepository.GetById(id);
        }

        public SisUsuario GetUserByName(string userName)
        {
            return userRepository.GetUserByName(userName);
        }

        public int UserInsert(SisUsuario user, int rolId)
        {
            return userRepository.UserInsert(user, rolId);
        }

        public int UserUpdate(SisUsuario user)
        {
            return userRepository.UserUpdate(user);
        }

        public bool UserExists(int id)
        {
            return userRepository.UserExists(id);
        }
        public bool UserDelete(int id)
        {
           // var user = userRepository.GetById(id);
            userRepository.Delete(id);
            return true;
        }

        public SisUsuario FindUser(string userName, string password)
        {
            var user = userRepository.FindUser(userName, password);
            return user;
        }

        public IEnumerable<string> GetUserPermission(int userId)
        {
            return userRepository.GetUserPermission(userId);
        }


    }
}
