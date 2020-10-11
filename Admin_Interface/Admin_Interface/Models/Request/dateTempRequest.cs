using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models.Request
{
    public class dateTempRequest
    {
        public int id { get; set; }
        public int id_marcacion { get; set; }
        public DateTime date { get; set; }
    }
}