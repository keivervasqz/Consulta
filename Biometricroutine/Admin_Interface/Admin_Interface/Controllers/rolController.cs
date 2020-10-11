using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Helpers;
using Admin_Interface.Models;
using Admin_Interface.Models.Request;

namespace Admin_Interface.Controllers
{
    public class rolController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<usuarios> _Usuarios;
        private PaginadorRoles<usuarios> _PaginadorUsuarios;

        // GET: rol
        public ActionResult Index(
            string usuario, string nombre,
            string cargo, string status, int p = 1)
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

                if (!string.IsNullOrEmpty(cargo))
                {
                    _Usuarios = _Usuarios.Where(x =>
                        x.id_rol.ToString().Contains(cargo.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(status))
                {
                    _Usuarios = _Usuarios.Where(x =>
                        x.status.ToString().Contains(status.ToLower())).ToList();
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
                return View(_PaginadorUsuarios);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: roles/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            ViewBag.rolesDescripcion = rolesDescripcion();
            return View();
        }

        // POST: roles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "pernr,nombre")] usuarios usuarios)
        {
            bool Status = false;
            string message = "";
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.DuplicateID = false;
                    usuarios search = db.usuarios.Find(usuarios.pernr);
                    if (search != null)
                    {
                        ViewBag.DuplicateID = true;
                        return View(usuarios);
                    }

                    db.usuarios.Add(usuarios);
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                    AuditoriaHelper.Auditoria(
                        oUser.pernr,
                        AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                        TipoAccion.Create, "Crear Rol",
                        AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, usuarios));
                   

                    return RedirectToAction("Index");
                }

                return View(usuarios);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Rol", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar crear un Rol.";
            }
            ViewBag.Message = message;
            Status = true;
            ViewBag.Status = Status;
            return View();
        }

        // GET: roles/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Roles = nombreRoles();
            ViewBag.rolesDescripcion = rolesDescripcion();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            usuarios rolUsuario = db.usuarios.Find(id);
            if (rolUsuario == null)
            {
                return HttpNotFound();
            }
            return View(rolUsuario);
        }

        // POST: roles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,lastname,email,id_rol,pernr,password,created_at,status")] usuarios usuario)
        {
            //bool Status = false;
            string message = "";
            try
            {
                ViewBag.rolesDescripcion = rolesDescripcion();
                ViewBag.roles = nombreRoles();
                //Auditoria 
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit, "Editar Rol",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.usuarios.Find(usuario.pernr), usuario));

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Rol", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar editar el rol.";
                ViewBag.Message = message;
                return View();

            }
        }

        // GET: sociedad/Delete/5
        public ActionResult Delete(string id)
        {
            string message = "";
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                usuarios usuarios = db.usuarios.Find(id);
                if (usuarios == null)
                {
                    return HttpNotFound();
                }
                return View(usuarios);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Borrar Rol", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar borrar el Rol.";
                ViewBag.Message = message;
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private List<usuarioRequest> nombreRoles()
        {
            List<usuarioRequest> list =
                (from d in db.usuarios
                 select new usuarioRequest
                 {
                     //id = d.id,
                     pernr = d.pernr,
                     name = d.name,
                     id_Rol = d.id_rol,
                     lastname = d.lastname,
                     email = d.email,
                     password = d.password,
                     created_at = d.created_at,
                     //status = d.status.ToString(),
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










        //// GET: rol
        //public ActionResult Index()
        //{
        //    return View(db.roles.ToList());
        //}

        //// GET: rol/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: rol/Create
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        //// más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "id,nivel,descripcion")] roles roles)
        //{
        //    if (ModelState.IsValid)

        //    {
        //        db.roles.Add(roles);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(roles);
        //}

        //// GET: rol/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    roles roles = db.roles.Find(id);
        //    if (roles == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(roles);
        //}

        //// POST: rol/Edit/5
        //// Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        //// más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "id,nivel,descripcion")] roles roles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(roles).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(roles);
        //}

        //// GET: rol/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    roles roles = db.roles.Find(id);
        //    if (roles == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(roles);
        //}

        //// POST: rol/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    roles roles = db.roles.Find(id);
        //    db.roles.Remove(roles);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
