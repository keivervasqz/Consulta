using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Admin_Interface.Helpers;
using Admin_Interface.Models;
using Admin_Interface.Models.Request;

namespace Admin_Interface.Controllers
{
    public class biometricoController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();
        private readonly int _RegistrosPorPagina = 10;
        private List<biometricos> _Biometrico;
        private PaginadorBiometrico<biometricos> _PaginadorBiometrico;

        // GET: biometrico
        public ActionResult Index(
            int? dirIp, string model, string subdiv,
            int? status, int p = 1)
        {
                        string message = "";
            try
            {
                int _TotalRegistros = 0;
                int _TotalPaginas = 0;

                _Biometrico = db.biometricos.ToList();

                if (!string.IsNullOrEmpty(dirIp.ToString()))
                {
                    _Biometrico = _Biometrico.Where(x =>
                            x.id.ToString().Contains(dirIp.ToString())).ToList();
                }

                if (!string.IsNullOrEmpty(model))
                {
                    _Biometrico = _Biometrico.Where(x =>
                            x.modelo.ToLower().Contains(model.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(subdiv))
                {
                    _Biometrico = _Biometrico.Where(x =>
                            x.id_subdivision.ToLower().Contains(subdiv.ToLower())).ToList();
                }

                if (status != null)
                {
                    _Biometrico = _Biometrico.Where(x =>
                            x.status.ToString().Contains(status.ToString())).ToList();
                }


                _TotalRegistros = _Biometrico.Count();
                _Biometrico = _Biometrico.OrderBy(x => x.id)
                            .Skip((p - 1) * _RegistrosPorPagina)
                            .Take(_RegistrosPorPagina)
                            .ToList();

                _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

                biometricoVar list = new biometricoVar()
                {
                    dirIp = dirIp.ToString(),
                    modelo = model,
                    subdivision = subdiv,
                    status = status.ToString()
                };


                _PaginadorBiometrico = new PaginadorBiometrico<biometricos>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _TotalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = p,
                    Resultado = _Biometrico,
                    tempVar = list
                };

                ViewBag.biometrico = db.biometricos.ToList();
                ViewBag.Subdivision = nombreSubdivision();
                return View(_PaginadorBiometrico);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: biometrico/Create
        public ActionResult configuracionBiometrico()
        {
            ViewBag.DuplicateID = false;
            return View();
        }

        // GET: biometrico/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            ViewBag.Subdivision = nombreSubdivision();
            return View();
        }

        // POST: biometrico/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,ip,modelo,id_subdivision,status")] biometricos biometricos)
        {
            string message = "";
            try { 
            ViewBag.Subdivision = nombreSubdivision();
            ViewBag.DuplicateID = false;
            biometricos search = db.biometricos.Where(d => d.ip == biometricos.ip).FirstOrDefault();
            if (search != null)
            {
                ViewBag.DuplicateID = true;
                return View(biometricos);
            }

           
            db.biometricos.Add(biometricos);
            //Auditoria 
            db.SaveChanges();
            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Create, "Crear Biometrico",
                AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, search, biometricos));
            

            return RedirectToAction("Index");
             }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crea Biometrico", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar crear el biometrico.";
            }
                ViewBag.Message = message;
                 return View();
            }

        // GET: biometrico/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Subdivision = nombreSubdivision();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            biometricos biometrico = db.biometricos.Find(id);
            if (biometrico == null)
            {
                return HttpNotFound();
            }
            return View(biometrico);
        }

        // POST: biometrico/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,ip,modelo,id_subdivision,status")] biometricos biometricos)
        {
            string message = "";
            try
            {
                ViewBag.Subdivision = nombreSubdivision();
            //Auditoria 
            var oUser = (usuarios)Session["User"];
                db.Entry(biometricos).State = EntityState.Modified;
                db.SaveChanges();
                AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Edit, "Editar Biometrico",
                AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.biometricos.Find(biometricos.id), biometricos));

            return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Biometrico", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar Editar los datos.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: biometrico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            biometricos biometrico = db.biometricos.Find(id);
            if (biometrico == null)
            {
                return HttpNotFound();
            }
            return View(biometrico);
        }

        // POST: biometrico/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            try
            {
                biometricos biometricos = db.biometricos.Find(id);
            //Auditoria 
            var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Delete, "Borrar Biometrico",
                AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name,biometricos,null));
            db.biometricos.Remove(biometricos);
            db.SaveChanges();
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina controlador", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
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

        private List<subdivisionRequest> nombreSubdivision()
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
