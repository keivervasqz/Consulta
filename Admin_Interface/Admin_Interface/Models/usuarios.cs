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
    
    public partial class usuarios
    {
        public string pernr { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public int id_rol { get; set; }
        public string password { get; set; }
        public System.DateTime created_at { get; set; }
        public int status { get; set; }
        public string sucursal { get; set; }
    }
}
