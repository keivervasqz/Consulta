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

namespace Admin_Interface.Controllers
{
    public class sociedadController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<sociedades> _Sociedad;
        private PaginadorSociedad<sociedades> _PaginadorSociedad;

        // GET: sociedad
        public ActionResult Index(
            string codigo, string nombre,
            int p = 1)
        {
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            _Sociedad = db.sociedades.ToList();

            if (!string.IsNullOrEmpty(codigo))
            {
                    _Sociedad = _Sociedad.Where(x =>
                x.id.ToLower().Contains(codigo.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                    _Sociedad = _Sociedad.Where(x =>
                x.nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }

            _TotalRegistros = _Sociedad.Count();
            _Sociedad = _Sociedad.OrderBy(x => x.id)
                        .Skip((p - 1) * _RegistrosPorPagina)
                        .Take(_RegistrosPorPagina)
                        .ToList();

            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

            sociedadVar list = new sociedadVar()
            {
                codigo = codigo,
                nombre = nombre
            };

            _PaginadorSociedad = new PaginadorSociedad<sociedades>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = p,
                Resultado = _Sociedad,
                tempVar = list
            };

            return View(_PaginadorSociedad);
        }

        // GET: division/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            return View();
        }

        // POST: sociedad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,nombre")] sociedades sociedades)
        {
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                ViewBag.DuplicateID = false;
                sociedades search = db.sociedades.Find(sociedades.id);
                if (search != null)
                {
                    ViewBag.DuplicateID = true;
                    return View(sociedades);
                }

                db.sociedades.Add(sociedades);
                //Auditoria 
                var oUser = (usuarios)Session["User"];
                    db.SaveChanges();
                    AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Create, "Crear Sociedad",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, sociedades));
               

                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Sociedad", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar crear la sociedad.";
            }
            ViewBag.Message = message;
            return View(sociedades);
        }

        // GET: sociedad/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sociedades sociedades = db.sociedades.Find(id);
            if (sociedades == null)
            {
                return HttpNotFound();
            }
            return View(sociedades);
        }

        // POST: sociedad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,nombre")] sociedades sociedades)
        {
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                    db.Entry(sociedades).State = EntityState.Modified;
                    db.SaveChanges();

                    AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit, "Editar Sociedad",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.sociedades.Find(sociedades.id), sociedades));
                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Sociedad", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al editar.";
            }
            ViewBag.Message = message;
            return View(sociedades);
       
           
        }

        // GET: sociedad/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sociedades sociedades = db.sociedades.Find(id);
            if (sociedades == null)
            {
                return HttpNotFound();
            }
            return View(sociedades);
        }

        // POST: sociedad/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            string message = "";
            try
            {
                sociedades sociedades = db.sociedades.Find(id);
                db.sociedades.Remove(sociedades);
                db.SaveChanges();
                //Auditoria 
                var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Delete, "Borrar Sociedad",
                AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, sociedades, null));

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar Sociedad", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al eliminar.";
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
    }
}
