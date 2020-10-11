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
    public class subdivisionController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<subdivisiones> _Subdivision;
        private PaginadorSubdivision<subdivisiones> _PaginadorSubdivision;

        // GET: subdivision
        public ActionResult Index(
            string codigo, string nombre,
            string division, int p = 1)

        {
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            _Subdivision = db.subdivisiones.ToList();

            if (!string.IsNullOrEmpty(codigo))
            {
                _Subdivision = _Subdivision.Where(x =>
                    x.id.ToLower().Contains(codigo.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(nombre))
            {
                _Subdivision = _Subdivision.Where(x =>
                    x.nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(division))
            {
                _Subdivision = _Subdivision.Where(x =>
                    x.id_division.ToLower().Contains(division.ToLower())).ToList();
            }
            
            _TotalRegistros = _Subdivision.Count();
            _Subdivision = _Subdivision.OrderBy(x => x.id)
                        .Skip((p - 1) * _RegistrosPorPagina)
                        .Take(_RegistrosPorPagina)
                        .ToList();

            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

            subdivisionVar list = new subdivisionVar()
            {
                codigo = codigo,
                nombre = nombre,
                division = division
            };

            _PaginadorSubdivision = new PaginadorSubdivision<subdivisiones>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = p,
                Resultado = _Subdivision,
                tempVar = list
            };

            ViewBag.Division = nombreDivision();
            return View(_PaginadorSubdivision);
        }

        // GET: subdivision/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            ViewBag.Division = nombreDivision();
            return View();
        }

        // POST: subdivision/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,nombre,id_division")] subdivisiones subdivisiones)
        {
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                ViewBag.DuplicateID = false;
                ViewBag.Division = nombreDivision();
                subdivisiones search = db.subdivisiones.Find(subdivisiones.id);
                if (search != null)
                {
                    ViewBag.DuplicateID = true;
                    return View(subdivisiones);
                }

                db.subdivisiones.Add(subdivisiones);
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Create, "Crear Subdivisiones",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, subdivisiones));

                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Subdivisión", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al crear la Subsivisión.";
            }
            ViewBag.Message = message;
            return View(subdivisiones);
        }

        // GET: subdivision/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.Division = nombreDivision();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subdivisiones subdivisiones = db.subdivisiones.Find(id);
            if (subdivisiones == null)
            {
                return HttpNotFound();
            }
            return View(subdivisiones);
        }

        // POST: subdivision/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,nombre,id_division")] subdivisiones subdivisiones)
        {
            string message = "";
            try
            {
                ViewBag.Division = nombreDivision();
                if (ModelState.IsValid)
                {
                    db.Entry(subdivisiones).State = EntityState.Modified;
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                    AuditoriaHelper.Auditoria(
                        oUser.pernr,
                        AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                        TipoAccion.Delete, "Editar Subdivisión",
                        AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, db.subdivisiones.Find(subdivisiones.id), subdivisiones));

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Subdivisión", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar editar.";
            }
            ViewBag.Message = message;
            return View(subdivisiones);      
            
        }

        // GET: subdivision/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subdivisiones subdivisiones = db.subdivisiones.Find(id);
            if (subdivisiones == null)
            {
                return HttpNotFound();
            }
            return View(subdivisiones);
        }

        // POST: subdivision/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            string message = "";
            try
            {
                subdivisiones subdivisiones = db.subdivisiones.Find(id);
            //Auditoria 
            var oUser = (usuarios)Session["User"];
                db.subdivisiones.Remove(subdivisiones);
                db.SaveChanges();
                AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Delete, "Eliminar Subdivisión",
                AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, subdivisiones, null));

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar Subdivisión", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar Eliminar.";
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

        private List<divisionRequest> nombreDivision()
        {
            List<divisionRequest> list =
                (from d in db.divisiones
                 select new divisionRequest
                 {
                     id = d.id,
                     nombre = d.nombre,
                     id_sociedad = d.id_sociedad
                 }).ToList();
            return list;
        }
    }
}
