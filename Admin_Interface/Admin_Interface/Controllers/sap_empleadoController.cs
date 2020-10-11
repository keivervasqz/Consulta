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
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace Admin_Interface.Controllers
{
    public class sap_empleadoController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<sap_empleados> _sapEmpleados;
        private Paginadorsap_Empleados<sap_empleados> _PaginadorSapEmpleados;

        // GET: sap_empleado
        public ActionResult Index(
            string pernr, string name,
            string status_empleado,
            string sucursal, int p = 1)
        {
            string message = "";
            try
            {
                int _TotalRegistros = 0;
                int _TotalPaginas = 0;

                _sapEmpleados = db.sap_empleados.ToList();

                if (!string.IsNullOrEmpty(pernr))
                {
                    _sapEmpleados = _sapEmpleados.Where(x =>
                x.pernr.ToLower().Contains(pernr.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(name))
                {
                    _sapEmpleados = _sapEmpleados.Where(x =>
                x.name.ToLower().Contains(name.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(status_empleado))
                {
                    _sapEmpleados = _sapEmpleados.Where(x =>
                x.status_empleado.ToString().Contains(status_empleado.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(sucursal))
                {
                    _sapEmpleados = _sapEmpleados.Where(x =>
                        x.sucursal.ToLower().Contains(sucursal.ToLower())).ToList();
                }

                _TotalRegistros = _sapEmpleados.Count();
                _sapEmpleados = _sapEmpleados.OrderBy(x => x.pernr)
                            .Skip((p - 1) * _RegistrosPorPagina)
                            .Take(_RegistrosPorPagina)
                            .ToList();

                _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

                sap_empleadoVar list = new sap_empleadoVar()
                {
                    pernr = pernr,
                    //name = name,
                    status = Convert.ToInt32(status_empleado),
                    sucursal = sucursal

                };

                _PaginadorSapEmpleados = new Paginadorsap_Empleados<sap_empleados>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _TotalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = p,
                    Resultado = _sapEmpleados,
                    tempVar = list
                };
                ViewBag.Subdivision = nombreSubdivision();
                return View(_PaginadorSapEmpleados);

            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Cargar pagina Empleados", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al intentar cargar la página del controlador.";
            }
            ViewBag.Message = message;
            return View();
        }
        
        // GET: sap_empleado/Create
        public ActionResult Create()
        {
            ViewBag.DuplicateID = false;
            ViewBag.Subdivision = nombreSubdivision();
            return View();
        }

        // POST: sap_empleado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id,pernr,name,lastname,dni,id_subdivision,created,permiso,status,status_empleado")] sap_empleados sap_empleados)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                try
                {
                    ViewBag.DuplicateID = false;
                    ViewBag.Subdivision = nombreSubdivision();
                    sap_empleados search = db.sap_empleados.Find(sap_empleados.id);
                    if (search != null)
                    {
                        ViewBag.DuplicateID = true;
                        return View(sap_empleados);
                    }

                    db.sap_empleados.Add(sap_empleados);
                    //Auditoria 
                    db.SaveChanges();
                    var oUser = (usuarios)Session["User"];
                    AuditoriaHelper.Auditoria(
                        oUser.pernr,
                        AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                        TipoAccion.Create, "Crear SAP Empleado",
                        AuditoriaHelper.ArmarDescription(TipoAccion.Create, oUser.name, null, sap_empleados));

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ErroresHelper.AgregarError(ex.Message, "Crear Empleado", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                    message = "Ocurrio un Error al crear el empleado.";
                }
                ViewBag.Message = message;
                return View(sap_empleados);
            }
            return View(sap_empleados);
        }

        // GET: sap_empleado/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.Subdivision = nombreSubdivision();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sap_empleados sap_empleados = db.sap_empleados.Find(id);
            if (sap_empleados == null)
            {
                return HttpNotFound();
            }
            return View(sap_empleados);
        }

        // POST: sap_empleado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,pernr,name,lastname,dni,id_subdivision,created,permiso,status,status_empleado,sucursal")] sap_empleados sap_empleados)
        {
            string message = "";
            try
            {
                ViewBag.Subdivision = nombreSubdivision();
            if (ModelState.IsValid)
            {
                    db.Entry(sap_empleados).State = EntityState.Modified;
                    db.SaveChanges();
                    //Auditoria 
                    var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Edit, "Editar SAP Empleado",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Edit, oUser.name, db.sap_empleados.Find(sap_empleados.id), sap_empleados));

                return RedirectToAction("Index");
            }
            return View(sap_empleados);
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Editar Datos Empleado", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
                message = "Ocurrio un Error al guardar los datos.";
            }
            ViewBag.Message = message;
            return View();
        }

        // GET: sap_empleado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            sap_empleados sap_empleados = db.sap_empleados.Find(id);
            if (sap_empleados == null)
            {
                return HttpNotFound();
            }
            return View(sap_empleados);
        }

        // POST: sap_empleado/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            string message = "";
            try
            {
                sap_empleados sap_empleados = db.sap_empleados.Find(id);
                db.sap_empleados.Remove(sap_empleados);
                db.SaveChanges();
                //Auditoria 
                var oUser = (usuarios)Session["User"];
                AuditoriaHelper.Auditoria(
                    oUser.pernr,
                    AuditoriaHelper.obtenerIp(Dns.GetHostName()),
                    TipoAccion.Delete, "Eliminar SAP Empleado",
                    AuditoriaHelper.ArmarDescription(TipoAccion.Delete, oUser.name, sap_empleados, null));

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Eliminar Empleado", AuditoriaHelper.obtenerIp(Dns.GetHostName()));
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
    }
}
