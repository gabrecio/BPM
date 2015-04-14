using System.ComponentModel.DataAnnotations.Schema;

namespace BPM.Models
{
    [Table("SisMenu")]
    public class SisMenu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Imagen { get; set; }
        public string Path { get; set; }
        public int OrderBy { get; set; }
    }
}
