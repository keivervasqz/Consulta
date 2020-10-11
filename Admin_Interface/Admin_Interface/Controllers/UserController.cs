using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
using Admin_Interface.Models.Request;
using Admin_Interface.Helpers;

namespace Admin_Interface.Controllers
{
    public class UserController : Controller
    {
        // TODO: CambiarNombre de La variable 
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<usuarios> _Usuarios;
        private PaginadorRoles<usuarios> _PaginadorUsuarios;

        // GET: user
        public ActionResult Index(
            string usuario, string nombre,
            string nivel_rol, string status,
            string sucursal, int p = 1)
        {
            string message = "";
            try
            {
                int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            _Usuarios = db.usuarios.ToList();

            if (!string.IsNullOrEmpty(usuario))
            {
                _Usuarios = _Usuarios.Where(x =>
            x.pernr.ToString().Contains(usuario.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                _Usuarios = _Usuarios.Where(x =>
            x.name.ToLower().Contains(nombre.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(nivel_rol))
            {
                _Usuarios = _Usuarios.Where(x =>
                    x.id_rol.ToString().Contains(nivel_rol.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                _Usuarios = _Usuarios.Where(x =>
                    x.status.ToString().Contains(status.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(sucursal))
            {
                _Usuarios = _Usuarios.Where(x =>
                    x.sucursal.ToLower().Contains(sucursal.ToLower())).ToList();
            }


                _TotalRegistros = _Usuarios.Count();
            _Usuarios = _Usuarios.OrderBy(x => x.pernr)
                        .Skip((p - 1) * _RegistrosPorPagina)
                        .Take(_RegistrosPorPagina)
                        .ToList();

            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

            UsuarioVar list = new UsuarioVar()
            {
                pernr = usuario,
                name = nombre

            };

            _PaginadorUsuarios = new PaginadorRoles<usuarios>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = p,
                Resultado = _Usuarios,
                tempVar = list
            };

            ViewBag.rolesDescripcion = rolesDescripcion();
            ViewBag.nombreSucursal = nombreSucursal();
            return View(_PaginadorUsuarios);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: user/Edit/5
        public ActionResult Edit(string id)
        {
            var v = db.usuarios.Where(a => a.pernr == id).FirstOrDefault();
            string pernr = v.pernr;
            ViewBag.user = nombreRoles();
            ViewBag.rolesDescripcion = rolesDescripcion();
            ViewBag.nombreSucursal = nombreSucursal();
            if (id == "0")
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            usuarios Usuario = db.usuarios.Find(pernr);


            if (Usuario == null)
            {
                return HttpNotFound();
            }
            return View(Usuario);
        }

        // POST: user/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,lastname,email,id_rol,pernr,password,created_at,status,sucursal")] usuarios usuario)
        {
            bool Status = false;
            string message = "";
            try
            {

                if (ModelState.IsValid)
                {
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                    AuditoriaHelper.Auditoria(
                    oUser.pernr,
                   AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                   TipoAccion.Edit, "Editar Usuario",
                   AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.usuarios.Find(usuario.pernr), usuario));
                    var usuarioEditar = db.usuarios.Find(usuario.pernr);
                    usuarioEditar.id_rol = usuario.id_rol;
                    usuarioEditar.lastname = usuario.lastname;
                    usuarioEditar.name = usuario.name;
                    usuarioEditar.status = usuario.status;
                    usuarioEditar.email = usuario.email;
                    usuarioEditar.sucursal = usuario.sucursal;
                    if (usuario.password != usuarioEditar.password)
                        usuarioEditar.password = Crypto.Hash(usuario.password);
                    else
                        usuarioEditar.password = usuario.password;
                    db.Entry(usuarioEditar).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                    return View(usuario);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar datos de usuario", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar editar los datos del usuario.";
                ViewBag.Message = message;

            }
            ViewBag.Message = message;
            Status = true;
            ViewBag.Status = Status;
            return View(usuario);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuarios user = db.usuarios.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            string message = "";
            try
            {
                var v = db.usuarios.Where(a => a.pernr == id).FirstOrDefault();
                string pernr = v.pernr;
                usuarios user = db.usuarios.Find(pernr);
                db.usuarios.Remove(user);
                db.SaveChanges();
                //Auditoria 
                var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Delete, "Eliminar Usuario",
                    AuditoriaHelper.ArmarDescription(
                        TipoAccion.Delete, oUser.name,
                        user, null));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar datos de usuario", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar eliminar los datos del usuario.";
                ViewBag.Message = message;

            }
            return RedirectToAction("Index");
        }


        //Prueba Keiver

        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            ViewBag.rolesDescripcion = rolesDescripcion();
            ViewBag.nombreSucursal = nombreSucursal();
            return View();
        }
        //Registration POST action 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Include = "id,name,lastname,email,id_rol,pernr,password,created_at,status,sucursal")] usuarios user)
        {
            bool Status = false;
            string message = "";
            try
            {
                ViewBag.rolesDescripcion = rolesDescripcion();
                ViewBag.nombreSucursal = nombreSucursal();
                //
                // Model Validation 
                if (ModelState.IsValid)
                {
                    #region  Password Hashing 
                    user.password = Crypto.Hash(user.password);
                    //user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                    #endregion

                    #region Save to Database
                    using (BIOMETRICOEntities dc = new BIOMETRICOEntities())
                    {
                        usuarios busca = dc.usuarios.Find(user.pernr);

                        if (busca == null)
                        {
                            user.created_at = DateTime.Today;
                            dc.usuarios.Add(user);
                            dc.SaveChanges();
                            //Auditoria    
                            var oUser = user;
                            var v = db.usuarios.Where(a => a.pernr == oUser.pernr).FirstOrDefault();
                            string pernr = v.pernr;
                            AuditoriaHelper.Auditoria(
                                oUser.pernr,
                                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                                TipoAccion.Register, "Registrar Usuario",
                                AuditoriaHelper.ArmarDescription(
                                    TipoAccion.Register, user.name,
                                    user, null));
                            message = " Se creó el usuario con éxito!!";
                            Status = true;

                            //AuditoriaHelper.Auditoria(
                            //  dc.usuarios.Find().pernr,
                            //    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                            //    TipoAccion.Register, "Usuario",
                            //    AuditoriaHelper.ArmarDescription(
                            //        TipoAccion.Register, user.name,
                            //        user, null));
                            //message = " Se creó el usuario con éxito!!"; 
                            //Status = true;
                        }
                        else
                        {
                            // DESPUES CREAS EL MESNAJE DE QUE EL PERNR YA EXISTE
                        }
                    }
                    #endregion
                }
                else
                {
                    message = "Error al crear usuario";
                }

                ViewBag.Message = message;
                ViewBag.Status = Status;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Usuario", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar crear un usuario.";
                ViewBag.Message = message;
                ViewBag.Status = Status;
                return View(user);
            }
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        private List<usuarioRequest> nombreRoles()
        {
            List<usuarioRequest> list =
                (from d in db.usuarios
                 select new usuarioRequest
                 {
 
                     pernr = d.pernr,
                     name = d.name,
                     id_Rol = d.id_rol,
                     lastname = d.lastname,
                     email = d.email,
                     password = d.password,
                     created_at = d.created_at,
                     status = d.status,
                 }).ToList();

            return list;
        }

        private List<rolesResquest> rolesDescripcion()
        {
            List<rolesResquest> list =
                (from d in db.roles
                 select new rolesResquest
                 {
                     id_rol = d.id_rol,
                     nivel_rol = d.nivel_rol,
                 }).ToList();
            return list;
        }


        public ActionResult searchEmpleado(string pernr)
        {
            //return Json("chamara", JsonRequestBehavior.AllowGet);3

            sap_empleados sap = db.sap_empleados.Where(x =>
                    x.pernr.ToLower() == pernr.ToLower()).FirstOrDefault();

            if (sap == null)
            {
                sap_empleados isap = new sap_empleados();
                isap.pernr = "";
                return Json(isap, JsonRequestBehavior.AllowGet);

            }

            return Json(sap, JsonRequestBehavior.AllowGet);
        }

        private List<subdivisionRequest> nombreSucursal()
        {
            List<subdivisionRequest> list =
                (from d in db.subdivisiones
                 select new subdivisionRequest
                 {
                     id = d.id,
                     nombre = d.nombre,
                     id_division = d.id_division
                 }).ToList();
            return list;
        }
    }
}
