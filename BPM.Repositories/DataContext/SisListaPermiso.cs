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
    
    public partial class SisListaPermiso
    {
        public SisListaPermiso()
        {
            this.SisRols = new HashSet<SisRol>();
        }
    
        public int lipId { get; set; }
        public int opId { get; set; }
        public Nullable<int> menuId { get; set; }
    
        public virtual SisMenu SisMenu { get; set; }
        public virtual SisOperacione SisOperacione { get; set; }
        public virtual ICollection<SisRol> SisRols { get; set; }
    }
}
