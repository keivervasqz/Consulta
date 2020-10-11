using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models.Security
{
    public class UserLoged
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Id_Rol { get; set; }
        public List<Pagina> Permisos { get; set; }
    }
}