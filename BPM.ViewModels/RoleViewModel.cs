using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using BPM.DataAcces;


namespace BPM.ViewModels
{
    public class RoleViewModel
    {
        
        public RoleViewModel() {
            Usuarios = new List<UserViewModel>();
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public List<UserViewModel> Usuarios { get; set; }
        public List<Permission> Permissions { get; set; }

      /*  public static Role For(SisRol rol)
        {
            Role RolDto = new Role();
            RolDto.Activo= rol.Activo;
            RolDto.Id = rol.rolId;
            RolDto.Nombre = rol.Nombre;
            RolDto.FechaAlta =  rol.FechaAlta;
            return RolDto;

        }

        public static SisRol For(Role RolDto)
        {
            SisRol rol = new SisRol();
            rol.Activo = RolDto.Activo;
            rol.rolId = RolDto.Id;
            rol.Nombre = RolDto.Nombre;
            rol.FechaAlta = (new DateTime(2010, 5, 1, 8, 30, 52).CompareTo(RolDto.FechaAlta) > 0) ? DateTime.Now : RolDto.FechaAlta;

            return rol;

        }*/
    }
}