using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models
{
    public class auditoriaVar
    {
        public int id { get; set; }
        public string idUsuario { get; set; }
        public string description { get; set; }
        public DateTime fechaModificacion { get; set; }
        public string proceso { get; set; }
        public string ip { get; set; }
        public int accion { get; set; }
    }
}