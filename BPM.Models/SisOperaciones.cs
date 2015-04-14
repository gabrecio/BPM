
using System.ComponentModel.DataAnnotations.Schema;

namespace BPM.Models
{
    [Table("SisOperaciones")]
    public class SisOperaciones
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int Id { get; set; }
       public string Nombre { get; set; }
       public string Imagen { get; set; }
       public int Orden { get; set; }
    }
}
