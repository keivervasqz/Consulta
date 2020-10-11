using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models.Request
{
    public class biometricoRequest
    {
        public int id { get; set; }
        public string ip { get; set; }
        public string modelo { get; set; }
        public string id_subdivision { get; set; }
        public bool status { get; set; }
    }
}