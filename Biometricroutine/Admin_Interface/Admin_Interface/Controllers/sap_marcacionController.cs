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
    public class sap_marcacionController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<sap_marcaciones> _Sap_marcacion;
        private PaginadorSap_marcacion<sap_marcaciones> _PaginadorSap_marcacion;

        // GET: sap_marcacion
        public ActionResult Index(
            string finicio, string ffin, string pernr, 
            int? dirIp, string in_out, string moment,
            int? status, int p = 1)
        {
            int _TotalRegistros = 0;
            int _TotalPaginas = 0;

            _Sap_marcacion = db.sap_marcaciones.ToList();

            if ((!string.IsNullOrEmpty(finicio)) && (!string.IsNullOrEmpty(ffin)))
            {
                DateTime fini = Convert.ToDateTime(finicio);
                DateTime ff = Convert.ToDateTime(ffin);
                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.fecha >= fini && x.fecha <= ff).ToList();
            }

            if (!string.IsNullOrEmpty(pernr))
            {
                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.pernr.Contains(pernr)).ToList();
            }

            if (!string.IsNullOrEmpty(dirIp.ToString()))
            {
                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.id_biometrico.ToString().Contains(dirIp.ToString())).ToList();
            }

            if (!string.IsNullOrEmpty(in_out))
            {
                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.zatza.Contains(in_out)).ToList();
            }

            if (!string.IsNullOrEmpty(moment))
            {
                if (moment == "0") moment = "";

                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.dallf == moment).ToList();
            }

            if (status != null)
            {
                _Sap_marcacion = _Sap_marcacion.Where(x =>
                    x.status.ToString().Contains(status.ToString())).ToList();
            }

            _TotalRegistros = _Sap_marcacion.Count();
            _Sap_marcacion = _Sap_marcacion.OrderBy(x => x.id)
                        .Skip((p - 1) * _RegistrosPorPagina)
                        .Take(_RegistrosPorPagina)
                        .ToList();

            _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

            sap_marcacionVar list = new sap_marcacionVar()
            {
                finicio = finicio,
                ffin = ffin,
                pernr = pernr,
                dirIp = dirIp.ToString(),
                in_out = in_out,
                moment = moment,
                status = status.ToString()
            };

            _PaginadorSap_marcacion = new PaginadorSap_marcacion<sap_marcaciones>()
            {
                RegistrosPorPagina = _RegistrosPorPagina,
                TotalRegistros = _TotalRegistros,
                TotalPaginas = _TotalPaginas,
                PaginaActual = p,
                Resultado = _Sap_marcacion,
                tempVar = list
            };

            ViewBag.biometrico = biometricoReq();
            //ViewBag.datoEmpleado = datoEmpleado();
            return View(_PaginadorSap_marcacion);
        }

        private object TruncateTime(object fechaCreacion)
        {
            throw new NotImplementedException();
        }

        // GET: sap_marcacion/Create
        public ActionResult Create()
        {
            ViewBag.biometrico = biometricoReq();
            return View();
        }

        // POST: sap_marcacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,id_biometrico,pernr,ldate,ltime,zatza,dallf,status,fulldata,fecha")] sap_marcaciones sap_marcaciones)
        {
            if (sap_marcaciones.dallf == null)
                sap_marcaciones.dallf = "";
            ViewBag.biometrico = biometricoReq();
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                db.sap_marcaciones.Add(sap_marcaciones);
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Create, "Crear SAP Marcacion",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, sap_marcaciones));

                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Crear Marcación", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error Crear la marcación.";
            }
            ViewBag.Message = message;
            return View(sap_marcaciones);
        }

        // GET: sap_marcacion/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.biometrico = biometricoReq();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sap_marcaciones sap_marcaciones = db.sap_marcaciones.Find(id);
            if (sap_marcaciones == null)
            {
                return HttpNotFound();
            }
            return View(sap_marcaciones);
        }

        // POST: sap_marcacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,id_biometrico,pernr,ldate,ltime,zatza,dallf,status,fulldata,fecha")] sap_marcaciones sap_marcaciones)
        {
            ViewBag.biometrico = biometricoReq();
            if (sap_marcaciones.dallf == null) sap_marcaciones.dallf = "";
            string message = "";
            try
            {
                if (ModelState.IsValid)
            {
                    db.Entry(sap_marcaciones).State = EntityState.Modified;
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit , "Editar SAP Marcacion",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.sap_marcaciones.Find(sap_marcaciones.id), sap_marcaciones));

                return RedirectToAction("Index");
            }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Marcación", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al editar los datos.";
            }
            ViewBag.Message = message;
            return View(sap_marcaciones);
            
        }

        // GET: sap_marcacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sap_marcaciones sap_marcaciones = db.sap_marcaciones.Find(id);
            if (sap_marcaciones == null)
            {
                return HttpNotFound();
            }
            return View(sap_marcaciones);
        }

        // POST: sap_marcacion/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            try
            {
                sap_marcaciones sap_marcaciones = db.sap_marcaciones.Find(id);
                db.sap_marcaciones.Remove(sap_marcaciones);
                db.SaveChanges();
                //Auditoria 
                var oUser = (usuarios)Session["User"];
            AuditoriaHelper.Auditoria(
                oUser.pernr,
                AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                TipoAccion.Delete, "Eliminar SAP Marcacion",
                AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, sap_marcaciones, null));

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar Marcaciónr", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
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

        private List<biometricoRequest> biometricoReq()
        {
            List<biometricoRequest> list =
                (from d in db.biometricos
                 select new biometricoRequest
                 {
                     id = d.id,
                     ip = d.ip
                 }).ToList();
            return list;
        }

        private DateTime formatFecha(string date)
        {
            DateTime fecha = new DateTime();
            string anio = date.Substring(0, 4);
            string mes = date.Substring(4, 6);
            string dia = date.Substring(6);
            string fec = anio + "-" + mes + "-" + dia;

            fecha = Convert.ToDateTime(fec);

            return fecha;
        }

        //private List<sap_empleadoRequest> datoEmpleado()
        //{
        //    List<sap_empleadoRequest> list =
        //        (from d in db.sap_empleados
        //         select new sap_empleadoRequest
        //         {
        //             pernr = d.pernr,
        //             name = d.name,
        //             lastname = d.lastname
        //         }).ToList();
        //    return list;
        //}
    }
}
