using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Models;
using System.Net;
using System.Web.Security;
using Admin_Interface.Models.Request;
using Admin_Interface.Models.Security;
using Admin_Interface.Helpers;
using Admin_Interface.Filters;

namespace Admin_Interface.Controllers
{
    public class AccesoController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();
        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            //Usamos la validación de MVC .NET
            if (!ModelState.IsValid)
            {
                login.password = string.Empty;
                return View(login);
            }


            string message = "";
            try
            {
                using (BIOMETRICOEntities dc = new BIOMETRICOEntities())
                {
                    #region MyRegion
                    //usuarios userDb = new usuarios();
                    //try
                    //{
                    //    userDb = dc.usuarios.Where(a => a.pernr == login.pernr).FirstOrDefault();

                    //}
                    //catch
                    //{
                    //    message = "El usuario no existe.";
                    //    login.password = string.Empty;
                    //    return View(login);
                    //}
                    //if (userDb.status.ToString() == "0")
                    //{
                    //    ViewBag.Message = "Su Usuario está inactivo";
                    //    return View();
                    //}
                    //if (userDb.status.ToString() == "2")
                    //{
                    //    ViewBag.Message = "Su Usuario está bloqueado";
                    //    return View();
                    //}
                    //if (string.Compare(Crypto.Hash(login.password), userDb.password) == 0)
                    //{
                    //    var oUser = userDb;
                    //    int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                    //    var ticket = new FormsAuthenticationTicket(login.pernr, login.RememberMe, timeout);
                    //    var userLog = new UserLoged();
                    //    userLog.Id = userDb.id;
                    //    userLog.Email = userDb.email;
                    //    userLog.Id_Rol = userDb.id_rol;
                    //    userLog.LastName = userDb.lastname;
                    //    userLog.Name = userDb.name;
                    //    userLog.Permisos = (from user in dc.usuarios
                    //                        join roles in dc.roles
                    //                        on user.id_rol equals roles.id_rol
                    //                        //para cuando tenga la tabla 
                    //                        // join pagina in dc.pagina
                    //                        //on rol.idrol equals pgina.rol
                    //                        select new Pagina
                    //                        {
                    //                            //Action = pagina.Accion,
                    //                            //Name = pagina.Nombre,
                    //                            //Controller = pagina.Controller
                    //                        })
                    //                        .ToList();
                    //    Session["User"] = oUser;
                    //    Session["User"] = userLog;
                    //    AuditoriaHelper.Auditoria();
                    //    return RedirectToAction("Index", "Home");
                    //}
                    //else
                    //{
                    //    message = "Usuario o contraseña Incorrectos.";
                    //}

                    #endregion 

                    #region Chequear 
                    var v = db.usuarios.Where(a => a.pernr == login.pernr).FirstOrDefault();
                    var oUser = v;
                    int nivelroln = v.id_rol;
                    var k = dc.roles.Where(i => i.id_rol == v.id_rol).FirstOrDefault();
                    //var s = dc.subdivisiones.Where(j => j.id == j.id).FirstOrDefault();
                    //Crear a cookie.

                    HttpCookie myCookie = new HttpCookie("idrol");
                    //Agregar llave-valor en la cookie.
                    myCookie.Values.Add("nivel_rol", k.nivel_rol.ToString());
                    //myCookie.Values.Add("sucursal", s.id.ToString());
                    //Establecer la fecha y hora de caducidad de las cookies. Hecho para durar las próximas 12 horas.
                    myCookie.Expires = DateTime.Now.AddHours(12);
                    //Lo más importante es escribir la cookie al cliente.
                    Response.Cookies.Add(myCookie);

                    if (v != null)
                    {
                        if (v.status.ToString() == "0")
                        {
                            ViewBag.Message = "Su Usuario está inactivo";
                            return View();
                        }
                        if (v.status.ToString() == "2")
                        {
                            ViewBag.Message = "Su Usuario está bloqueado";
                            return View();
                        }
                        if (v.id_rol.ToString() == "5")
                        {
                            ViewBag.Message = "Usuario no autorizado";
                            return View();
                        }

                        if (string.Compare(Crypto.Hash(login.password), v.password) == 0)
                        {
                            int timeout = login.RememberMe ? 525600 : 200; // 525600 min = 1 year
                            var ticket = new FormsAuthenticationTicket(login.pernr.ToString(), login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);
                            Session["User"] = oUser;
                            //Auditoria 
                            AuditoriaHelper.Auditoria(
                                v.pernr,
                                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                                TipoAccion.Login, "Inicio de sesión",
                                AuditoriaHelper.ArmarDescription(TipoAccion.Login,oUser.name+" "+oUser.lastname));
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("principal", "Acceso");
                            }
                        }
                        else
                        {
                            message = "Usuario o contraseña invalida";
                        }
                    }
                    else
                    {
                        message = "Usuario o contraseña invalida";
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Login",AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error intente de nuevo.";
            }
            ViewBag.Message = message;
            return View();
        }

        

        //Logout
       // [Authorize]
       [VerificaSession]
        [HttpPost]
        public ActionResult Logout()
        {

           var  oUser  = (usuarios)Session["User"];
            //Auditoria 
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Login, "Fin de sesión",
                AuditoriaHelper.ArmarDescription(TipoAccion.Login, oUser.name + " " + oUser.lastname));

            FormsAuthentication.SignOut();

            Session.Abandon();
            return RedirectToAction("Login", "User");
        }

        public ActionResult principal()
        {
            ViewBag.Title = "Inicio";

            return View();
        }

        private List<rolesResquest> rolesDescripcion()
        {
            List<rolesResquest> list =
                (from d in db.roles
                 select new rolesResquest
                 {
                     id_rol = d.id_rol,
                     nivel_rol = d.nivel_rol

                 }
                 ).ToList();
            return list;
        }

       

    }
}