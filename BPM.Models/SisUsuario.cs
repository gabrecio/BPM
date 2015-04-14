
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BPM.Models
{

        [Table("SisUsuario")]
        public class SisUsuario : IdentityUser<int, UserLoginIntPk, UserRoleIntPk, UserClaimIntPk>
        {

           /* [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }*/

            public SisUsuario()
            : base()
        {
           // FilmHeadersCounted = new HashSet<FilmHeader>();
            //FilmHeadersSigned = new HashSet<FilmHeader>();
        }

            public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SisUsuario, int> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public DateTime CreatedDate { get; set; }

        //public virtual ICollection<FilmHeader> FilmHeadersSigned { get; set; }

        //public virtual ICollection<FilmHeader> FilmHeadersCounted { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Mail { get; set; } 
            public string Password { get; set; }
           
        }

        //New drived classes 
        public class UserRoleIntPk : IdentityUserRole<int>
        {
        }

        public class UserClaimIntPk : IdentityUserClaim<int>
        {
        }

        public class UserLoginIntPk : IdentityUserLogin<int>
        {
        }

        public class RoleIntPk : IdentityRole<int, UserRoleIntPk>
        {
            public RoleIntPk() { }
            public RoleIntPk(string name) { Name = name; }
        }
    
}
