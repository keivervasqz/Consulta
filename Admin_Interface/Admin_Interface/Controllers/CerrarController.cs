using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Models;
using System.Net;
using System.Web.Security;
using Admin_Interface.Models.Request;
using Admin_Interface.Helpers;


namespace Admin_Interface.Controllers
{
    public class CerrarController : Controller
    {
        // GET: Cerrar
        public ActionResult Logout()
        {
            string message = "";
            try
            {
                FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Acceso");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cerrar sesión", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al cerrar la sesión.";
            }
            ViewBag.Message = message;
            return View();
        }
    }
}