using System.ComponentModel.DataAnnotations.Schema;

namespace BPM.Models
{
    [Table("SisRol")]
    public class SisRol: BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
