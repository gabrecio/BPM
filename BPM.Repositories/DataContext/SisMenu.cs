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
    
    public partial class SisMenu
    {
        public SisMenu()
        {
            this.SisListaPermisoes = new HashSet<SisListaPermiso>();
        }
    
        public int menuId { get; set; }
        public string Clave { get; set; }
        public string Imagen { get; set; }
        public string Titulo { get; set; }
        public string Path { get; set; }
        public string OrderBy { get; set; }
    
        public virtual ICollection<SisListaPermiso> SisListaPermisoes { get; set; }
    }
}