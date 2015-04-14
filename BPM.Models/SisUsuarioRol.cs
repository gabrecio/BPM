using System.ComponentModel.DataAnnotations.Schema;
namespace BPM.Models
{
     [Table("SisUsuarioRol")]
    public class SisUsuarioRol
    {
        public virtual SisRol RolId { get; set; }
        public virtual SisUsuario UsuarioId { get; set; }
    }
}
