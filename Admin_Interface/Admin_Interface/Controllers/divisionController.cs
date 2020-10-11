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
    public class divisionController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<divisiones> _Divisiones;
        private PaginadorDivision<divisiones> _PaginadorDivision;

        // GET: division
        public ActionResult Index(
            string codigo, string nombre,
            string sociedad, string status,int p = 1)
        {
            string message = "";
            try
            {
                int _TotalRegistros = 0;
                int _TotalPaginas = 0;

                _Divisiones = db.divisiones.ToList();

                if (!string.IsNullOrEmpty(codigo))
                {
                    _Divisiones = _Divisiones.Where(x =>
                        x.id.ToLower().Contains(codigo.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(nombre))
                {
                    _Divisiones = _Divisiones.Where(x =>
                        x.nombre.ToLower().Contains(nombre.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(sociedad))
                {
                    _Divisiones = _Divisiones.Where(x =>
                        x.id_sociedad.ToLower().Contains(sociedad.ToLower())).ToList();
                }

                _TotalRegistros = _Divisiones.Count();
                _Divisiones = _Divisiones.OrderBy(x => x.id)
                            .Skip((p - 1) * _RegistrosPorPagina)
                            .Take(_RegistrosPorPagina)
                            .ToList();

                _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

                divisionVar list = new divisionVar()
                {
                    codigo = codigo,
                    nombre = nombre,
                    sociedad = sociedad
                };

                _PaginadorDivision = new PaginadorDivision<divisiones>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _TotalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = p,
                    Resultado = _Divisiones,
                    tempVar = list
                };

                ViewBag.Sociedad = nombreSociedad();
                return View(_PaginadorDivision);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: division/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            ViewBag.Sociedad = nombreSociedad();
            return View();
        }

        // POST: division/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,nombre,id_sociedad")] divisiones divisiones)
        {
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                ViewBag.DuplicateID = false;
                ViewBag.Sociedad = nombreSociedad();
                divisiones search = db.divisiones.Find(divisiones.id);
                if (search != null)
                {
                    ViewBag.DuplicateID = true;
                    return View(divisiones);
                }

                db.divisiones.Add(divisiones);
              db.SaveChanges();
                    //Auditoria 
              var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Create, "Crear División",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name,null,divisiones));
              
                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear División", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar crear la División.";
            }
            ViewBag.Message = message;
            return View(divisiones);
        }

        // GET: division/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.Sociedad = nombreSociedad();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            divisiones divisiones = db.divisiones.Find(id);
            if (divisiones == null)
            {
                return HttpNotFound();
            }
            return View(divisiones);
        }

        // POST: division/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,nombre,id_sociedad")] divisiones divisiones)
        {
            string message = "";
            try
            {
                ViewBag.Sociedad = nombreSociedad();
            if (ModelState.IsValid)
            {
                //Auditoria 
                var oUser = (usuarios)Session["User"];
                    db.Entry(divisiones).State = EntityState.Modified;
                    db.SaveChanges();
                    AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit, "Editar División",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name,db.divisiones.Find(divisiones.id), divisiones));

                return RedirectToAction("Index");
            }
            return View(divisiones);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View(divisiones);
        }

        // GET: division/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            divisiones division = db.divisiones.Find(id);
            if (division == null)
            {
                return HttpNotFound();
            }
            return View(division);
        }

        // POST: division/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            string message = "";
            try
            {
                divisiones divisiones = db.divisiones.Find(id);
                //Auditoria 
                var oUser = (usuarios)Session["User"];
                db.divisiones.Remove(divisiones);
                db.SaveChanges();
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Delete, "Eliminar División",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, divisiones, null));

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar División", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar eliminar.";
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

        private List<sociedadRequest> nombreSociedad()
        {
            List<sociedadRequest> list =
                (from d in db.sociedades
                 select new sociedadRequest
                 {
                     id = d.id,
                     nombre = d.nombre
                 }).ToList();
            return list;
        }
    }
}
