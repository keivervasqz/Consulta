using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models
{
    public class sap_empleadoVar
    {
        public int id { get; set; }
        public string pernr { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string depto { get; set; }
        public string sucursal { get; set; }
        public string dni { get; set; }
        public Nullable<System.DateTime> created { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> status_empleado { get; set; }
    }
}