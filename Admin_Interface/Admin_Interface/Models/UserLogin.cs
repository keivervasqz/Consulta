
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Admin_Interface.Models
{
    public class UserLogin
    {
        //[Display(Name = "Usuario requerido")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Usuario requerido")]
        [Required(ErrorMessage ="El {0} es Requerido")]
        [Display(Name ="Usuario")]
        public string pernr { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Contraseña requerida")]
        [Required(ErrorMessage = "El {0} es Requerido")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        //[Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}