using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Admin_Interface.Models;
using Admin_Interface.Helpers;
using TaskScheduler;
using System.Configuration;

namespace Admin_Interface.Controllers
{
    [AllowAnonymous]
    public class UserBiometricController : Controller
    {
        BIOMETRICOEntities db = new BIOMETRICOEntities();

        // GET: UserBiometric
        public ActionResult Index()
        {
            cargarDropDownlistBiometricos();

            return View();
        }

        public ActionResult empleadosBiometricos()
        {
            cargarDropDownlistBiometricos();

            return View();
        }

        public ActionResult trasladarEmpleado()
        {
            cargarDropDownlistBiometricos();

            return View();
        }

        public ActionResult programarMarcaciones()
        {
            cargarDropDownlistBiometricos();

            return View();
        }

        #region Pasamos los Datos a la Vista
        private void cargarDropDownlistBiometricos()
        {
            ViewBag.ListaUser = CargarUser();
            ViewBag.ListaBiometricos = CargarBiometricos();
            ViewBag.ListaSucursales = CargarSucursales();
        }

        #endregion

        #region Cargamos los datos de los DropDownList
        private List<SelectListItem> CargarUser()
        {
            List<SelectListItem> resultado = db.sap_empleados.ToList().OrderBy(x => x.pernr).
                Select(x => new SelectListItem
                {
                    Text = "(" + x.pernr + ") " + x.name + " " + x.lastname,
                    Value = x.pernr
                }).ToList();

            return resultado;
        }

        private List<SelectListItem> CargarBiometricos()
        {
            List<SelectListItem> resultado = db.biometricos.ToList().Select(x => 
            new SelectListItem
            {
                Text = x.ip,
                Value = x.id.ToString()
            }).ToList();

            return resultado;
        }

        private List<SelectListItem> CargarSucursales()
        {
            List<SelectListItem> resultado = db.subdivisiones.ToList().Select(x =>
            new SelectListItem
            {
                Text = x.nombre,
                Value = x.id.ToString()
            }).ToList();

            return resultado;
        }
        #endregion

        #region SetAllUser
        [HttpPost]
        public ActionResult SetAllUser(string sucursal, int ListBiometricos)
        {
            string Mensaje = "Se adiciono "; bool response = true;

            try
            {
                if (ListBiometricos == 0) // Todos los Biometricos pero...
                {
                    if ((!string.IsNullOrEmpty(sucursal))) // Todos las ip's de la sucursal...
                    {
                        if (!BiometricSoft("SetAllUser", sucursal))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                    else // ...o en todas las sucursales.
                    {
                        if (!BiometricSoft("SetAllUser"))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                }
                else
                {   // Solo recorre un Biometrico.

                    biometricos biometrico = db.biometricos.Where(x =>
                        x.id == ListBiometricos).FirstOrDefault();

                    if (!BiometricSoft("SetAllUser", "", biometrico.ip))
                    {
                        // Logs error NO SE PUDO OBTENER MARCACIONES
                        response = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response = false;
                ErroresHelper.AgregarError(ex.Message, "UserBiometric", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
            }


            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Create, "Registrar-Actualizar Usuario",
                Mensaje);

            ViewBag.response = response;
            return RedirectToAction("Index");
        }
        #endregion

        #region GetAllUser
        [HttpPost]
        public ActionResult GetAllUser(int ListBiometricos, string sucursal)
        {
            string Mensaje = "Se adicionó "; bool response = true;

            try
            {
                if (ListBiometricos == 0) // Todos los Biometricos pero...
                {   
                    if ((!string.IsNullOrEmpty(sucursal))) // Todos los de la sucursal...
                    {
                        if (!BiometricSoft("GetAllUser", sucursal))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                    else // ...o todos en todas las sucursales.
                    {
                        if (!BiometricSoft("GetAllUser"))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                }
                else
                {   // Solo recorre un Biometrico.

                    biometricos biometrico = db.biometricos.Where(x => 
                        x.id == ListBiometricos).FirstOrDefault();

                    if (!BiometricSoft("GetAllUser", "", biometrico.ip))
                    {
                        // Logs error NO SE PUDO OBTENER MARCACIONES
                        response = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response = false;
                ErroresHelper.AgregarError(ex.Message, "UserBiometric", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
            }

            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Create, "Guardar Usuario en BDO",
                Mensaje);

            ViewBag.response = response;
            return RedirectToAction("Index");
        }
        #endregion

        #region SetUser
        [HttpPost]
        public ActionResult SetUser(string sucursal, string pernr)
        {
            string Mensaje = "Se adiciono "; bool response = true;

            try
            {
                if (!string.IsNullOrEmpty(sucursal)) // Todos las ip's de la sucursal...
                {
                    if (!BiometricSoft("SetUser", sucursal, pernr))
                    {
                        // Logs error NO SE PUDO OBTENER MARCACIONES
                        response = false;
                    }
                }
                else // ...o en todas las sucursales.
                {
                    if (!BiometricSoft("SetUser", "", pernr))
                    {
                        // Logs error NO SE PUDO OBTENER MARCACIONES
                        response = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response = false;
                ErroresHelper.AgregarError(ex.Message, "UserBiometric", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
            }
            

            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Create, "Registrar-Actualizar Usuario",
                Mensaje);

            ViewBag.response = response;
            return RedirectToAction("Index");
        }

        #endregion

        #region GetMarcaciones
        [HttpPost]
        public ActionResult GetMarcaciones(int marca, string sucursal, string dia, string hora)
        {
            string Mensaje = "Se adicionó "; bool response = true;

            try
            {
                if (marca == 0) // Ejecutar Inmediatamente
                {
                    if (!string.IsNullOrEmpty(sucursal)) // Todos las ip's de la sucursal...
                    {
                        if (!BiometricSoft("GetMarcaciones", sucursal))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                    else // En todas la sucursales...
                    {
                        if (!BiometricSoft("GetMarcaciones"))
                        {
                            // Logs error NO SE PUDO OBTENER MARCACIONES
                            response = false;
                        }
                    }
                }
                else
                {
                    if (!Task(marca, dia, hora))
                    {
                        // Logs error NO SE PUDO OBTENER MARCACIONES
                        response = false;
                    }
                }
            }
            catch (Exception ex)
            {
                response = false;
                ErroresHelper.AgregarError(ex.Message, "UserBiometric", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
            }


            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Create, "Guardar Marcación",
                Mensaje);

            ViewBag.response = response;
            return RedirectToAction("Index");
        }

        #endregion

        private bool BiometricSoft(string option, string sucursal = "", string search = "")
        {
            bool result = true;

            ProcessStartInfo startInfo = new ProcessStartInfo();

            startInfo.FileName = "C:\\Users\\jhonriv\\AppData\\Local\\Apps\\2.0\\GGLOC4TE.2J5\\7B38L844.8HP\\biom..tion_727c74f158f13954_0001.0000_3c366e4f56ad4413\\Biometricroutine.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = option + " " + sucursal + " " + search;
            
            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        private bool Task(int marca, string dia, string hora)
        {
            bool result = false;

            using (ScheduledTasks Tareas = new ScheduledTasks())

            {
                Task tarea = Tareas.CreateTask("BiometricSoft_Task");

                // archivo que vamos a ejecutar, escribimos la ruta completa

                tarea.ApplicationName = @"C:\\Users\\jhonriv\\AppData\\Local\\Apps\\2.0\\GGLOC4TE.2J5\\7B38L844.8HP\\biom..tion_727c74f158f13954_0001.0000_3c366e4f56ad4413\\Biometricroutine.exe";

                tarea.Comment = "Programador de Marcaciones";

                // configurar la cuenta con la que se ejecutara la tarea

                try
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    string win_user = appSettings["win_user"] ?? "";
                    string win_pass = appSettings["win_pass"] ?? "";

                    tarea.SetAccountInformation(win_user, win_pass);
                }
                catch (ConfigurationErrorsException)
                {
                    
                }
                
                // limitar la duración de la tarea programada

                tarea.MaxRunTime = new TimeSpan(0, 15, 0);

                tarea.Creator = "Cronapis";

                // prioridad de la tarea

                tarea.Priority = ProcessPriorityClass.Normal;

                // agregamos el disparador

                short horas = Convert.ToInt16(hora.Substring(0, 2));
                short minutos = Convert.ToInt16(hora.Substring(2, 4));
                short interval = getmarca(marca);

                tarea.Triggers.Add(new WeeklyTrigger(horas, minutos, selectdia(dia), interval));

                //int[] dias = new int[] { 1, 15 };
                //tarea.Triggers.Add(new MonthlyTrigger(18, 15, dias));

                tarea.Save();

            }


            return result;
        }

        private DaysOfTheWeek selectdia(string dia)
        {
            DaysOfTheWeek week = DaysOfTheWeek.Sunday;
            switch (dia)
            {
                case "Lunes":
                    week = DaysOfTheWeek.Monday;
                    break;
                case "Martes":
                    week = DaysOfTheWeek.Tuesday;
                    break;
                case "Miercoles":
                    week = DaysOfTheWeek.Wednesday;
                    break;
                case "Jueves":
                    week = DaysOfTheWeek.Thursday;
                    break;
                case "Viernes":
                    week = DaysOfTheWeek.Friday;
                    break;
                case "Sabado":
                    week = DaysOfTheWeek.Saturday;
                    break;
            }

            return week;
        }

        private short getmarca(int marca)
        {
            short interval = 0;

            switch (marca)
            {
                case 7:
                    interval = 1;
                    break;
                case 15:
                    interval = 2;
                    break;
                case 30:
                    interval = 4;
                    break;
            }

            return interval;
        }
    }
}