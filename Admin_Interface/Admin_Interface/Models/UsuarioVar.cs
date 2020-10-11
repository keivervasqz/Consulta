using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models
{
    public class UsuarioVar
    {
        //public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public int id_Rol { get; set; }
        public string pernr { get; set; }
        public string password { get; set; }
        public System.DateTime created_at { get; set; }
        public int status { get; set; }
        public string sucursal { get; set; }
    }
}