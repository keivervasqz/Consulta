using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Filters;
using System.Net;
using Admin_Interface.Models;
using Admin_Interface.Models.Request;

namespace Admin_Interface.Controllers
{
    public class HomeController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();
        public ActionResult Index()
        {
            ViewBag.Title = "Inicio";
            return View();
        }


        public ActionResult Rol()
        {
            ViewBag.Title = "Roles";

            return View();
        }


        public ActionResult Sociedad()
        {
            ViewBag.Title = "Sociedades";
            
            return View();
        }

        public ActionResult Division()
        {
            ViewBag.Title = "Divisiones";

            return View();
        }


        public ActionResult Subdivision()
        {
            ViewBag.Title = "Subdivisiones";

            return View();
        }

        public ActionResult Empleado()
        {
            ViewBag.Title = "Empleados";

            return View();
        }

        public ActionResult Biometrico()
        {
            
            
            return View();
        }
        public ActionResult Marcacion()
        {
            ViewBag.Title = "Marcaciones";

            return View();
        }

        public ActionResult Usuario()
        {
            ViewBag.Title = "Usuario";

            return View();
        }
    }
}
