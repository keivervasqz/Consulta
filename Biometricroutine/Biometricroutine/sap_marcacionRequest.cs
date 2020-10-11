using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biometricroutine
{
    class sap_marcacionRequest
    {
        public int id { get; set; }
        public int id_biometrico { get; set; }
        public string pernr { get; set; }
        public string ldate { get; set; }
        public string ltime { get; set; }
        public string zatza { get; set; }
        public string dallf { get; set; }
        public int status { get; set; }
        public string fulldata { get; set; }
        public DateTime fecha { get; set; }

    }
}
