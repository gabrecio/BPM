using System;
using System.Collections.Generic;




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
       
    }
}