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
    public class roles_UsuarioController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<roles> _Usuarios;
        private PaginadorRoles_Usuarios<roles> _PaginadorUsuarios;

        // GET: rol
        public ActionResult Index(
            int? id_rol, string rol_descripion,
            int? nivel_rol, int p = 1)
        {
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            _Usuarios = db.roles.ToList();

            if (!string.IsNullOrEmpty(id_rol.ToString()))
            {
                _Usuarios = _Usuarios.Where(x =>
            x.id_rol.ToString().Contains(id_rol.ToString())).ToList();
            }

            //if (!string.IsNullOrEmpty(rol_descripion))
            //{
            //    _Usuarios = _Usuarios.Where(x =>
            //x.rol_descripcion.ToLower().Contains(rol_descripion.ToLower())).ToList();
            //}

            if (!string.IsNullOrEmpty(nivel_rol.ToString()))
            {
                _Usuarios = _Usuarios.Where(x =>
                    x.nivel_rol.ToString().Contains(nivel_rol.ToString())).ToList();
            }

            _TotalRegistros = _Usuarios.Count();
            _Usuarios = _Usuarios.OrderBy(x => x.id_rol)
                        .Skip((p - 1) * _RegistrosPorPagina)
                        .Take(_RegistrosPorPagina)
                        .ToList();

            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

            rolVar list = new rolVar()
            {
                id_rol = id_rol.ToString(),
                rol_descripcion = rol_descripion,

            };

            _PaginadorUsuarios = new PaginadorRoles_Usuarios<roles>()
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
        public ActionResult Create([Bind(Include = "id_rol,rol_descripcion,nivel_rol")] roles roles)
        {
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                ViewBag.DuplicateID = false;
                roles search = db.roles.Find(roles.id_rol);
                if (search != null)
                {
                    ViewBag.DuplicateID = true;
                    return View(roles);
                }

                db.roles.Add(roles);
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Create, "Crear Rol",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, roles));

                return RedirectToAction("Index");
            }

            return View(roles);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Rol", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al crear el rol.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: roles/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Roles = nombreRoles();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            roles rolUsuario = db.roles.Find(id);
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
        public ActionResult Edit([Bind(Include = "rol_descripcion, nivel_rol, id_rol")] roles roles)
        {
            string message = "";
            try
            {
                ViewBag.roles = rolesDescripcion();
            if (ModelState.IsValid)
            {
                    //Auditoria 
                    db.Entry(roles).State = EntityState.Modified;
                    db.SaveChanges();
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit, "Editar Rol",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name,db.roles.Find(roles.id_rol),roles));

                return RedirectToAction("Index");
            }
            return View(roles);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();

        }

        // GET: sociedad/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            roles roles = db.roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            try
            {
                roles roles = db.roles.Find(id);
            db.roles.Remove(roles);
                db.SaveChanges();
                //Auditoria 
                var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Delete, "Rol usuario",
                AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, roles, null));
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar Rol", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al eliminar el rol.";
            }
            ViewBag.Message = message;
            return View();
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
                     id_Rol = d.id_rol
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


    }
}