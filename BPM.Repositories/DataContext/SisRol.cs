//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BPM.Repositories.DataContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class SisRol
    {
        public SisRol()
        {
            this.SisListaPermisoes = new HashSet<SisListaPermiso>();
            this.SisUsuarios = new HashSet<SisUsuario>();
        }
    
        public int rolId { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public System.DateTime FechaAlta { get; set; }
    
        public virtual ICollection<SisListaPermiso> SisListaPermisoes { get; set; }
        public virtual ICollection<SisUsuario> SisUsuarios { get; set; }
    }
}
