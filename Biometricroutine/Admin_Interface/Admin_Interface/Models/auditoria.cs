//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Admin_Interface.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class auditoria
    {
        public int id { get; set; }
        public string idUsuario { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> fechaModificacion { get; set; }
        public string proceso { get; set; }
        public string ip { get; set; }
        public Nullable<int> accion { get; set; }
    }
}
