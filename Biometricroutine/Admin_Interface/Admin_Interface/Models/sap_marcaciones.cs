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
    
    public partial class sap_marcaciones
    {
        public int id { get; set; }
        public string pernr { get; set; }
        public string ldate { get; set; }
        public string ltime { get; set; }
        public string zatza { get; set; }
        public string dallf { get; set; }
        public Nullable<int> status { get; set; }
        public string fulldata { get; set; }
        public Nullable<int> id_biometrico { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
        public Nullable<System.DateTime> date_capture { get; set; }
    }
}
