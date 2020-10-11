using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models.Request
{
    public class sap_marcacionRequest
    {
        public int id { get; set; }
        public int id_biometrico { get; set; }
        public string pernr { get; set; }
        public string ldate { get; set; }
        public string ltime { get; set; }
        public string zatza { get; set; }
        public string dalf { get; set; }
        public bool status { get; set; }
        public string fulldata { get; set; }
    }
}