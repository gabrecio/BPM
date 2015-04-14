using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPM.ViewModels;



namespace BPM.ViewModels
{
    public class UserViewModel
    {
      /*  public User()
        {
            roles = new List<Rol>();
        }*/
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Password { get; set; }
        public List<RoleViewModel> roles  { get; set; }

      /*  public static User For(SisUsuario usuario)
        {
            User item = new User();
                item.Id = usuario.usuarioId;
                item.Nombre = usuario.Nombre;
                item.Apellido = usuario.Apellido;
                item.Mail = usuario.Mail;
                item.Activo = usuario.Activo;
                item.FechaAlta = usuario.FechaAlta;
                item.roles = new List<Role>();
                if (usuario.SisRols  != null )
                {                   
                    foreach (SisRol rol in usuario.SisRols)
                    {
                        item.roles.Add(new Role()
                        {
                            Nombre = rol.Nombre,
                            Id = rol.rolId,
                            Activo = rol.Activo
                        });                      
                    }
                }         
            
            return item;
        }
        public static SisUsuario For(User usuario)
        {                       
            var h = new Hasher { SaltSize = 16 };
            SisUsuario item = new SisUsuario();
            item.usuarioId = usuario.Id;
            item.Nombre = usuario.Nombre;
            item.Apellido = usuario.Apellido;
            item.Mail = usuario.Mail;
            item.Activo = usuario.Activo ;
            item.FechaAlta = (new DateTime(2010, 5, 1, 8, 30, 52).CompareTo(usuario.FechaAlta) > 0) ? DateTime.Now : usuario.FechaAlta;
            item.Password = usuario.Password;
            return item;
        }*/
    }
}