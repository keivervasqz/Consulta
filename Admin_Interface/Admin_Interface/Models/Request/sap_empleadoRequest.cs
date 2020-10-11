using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models.Request
{
    public class sap_empleadoRequest
    {
        public int id { get; set; }
        public string pernr { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string dni { get; set; }
        public string id_subdivision { get; set; }
        public DateTime created { get; set; }
        public string permiso { get; set; }
        public int status { get; set; }
        public int status_empleado { get; set; }
    }
}