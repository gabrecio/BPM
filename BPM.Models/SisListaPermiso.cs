using System.ComponentModel.DataAnnotations.Schema;

namespace BPM.Models
{
     [Table("SisListaPermiso")]
    public class SisListaPermiso
    {
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public int Id { get; set; }
         public virtual SisOperaciones OpId { get; set; }
         public virtual SisMenu MenuId { get; set; }
    }
}
