using System.ComponentModel.DataAnnotations.Schema;

namespace BPM.Models
{
    [Table("SisRolPermiso")]
    public class SisRolPermiso
    {
         public virtual SisRol RolId { get; set; }
         public virtual SisListaPermiso LipId { get; set; }
    }
}
