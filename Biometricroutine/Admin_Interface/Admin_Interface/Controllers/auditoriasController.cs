using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Models;
using Admin_Interface.Models.Request;
using Admin_Interface.Helpers;

namespace Admin_Interface.Controllers
{
    public class auditoriasController : Controller
    {
        private BIOMETRICOEntities db = new BIOMETRICOEntities();

        private readonly int _RegistrosPorPagina = 10;
        private List<auditoria> _Auditoria;
        private PaginadorAuditoria<auditoria> _PaginadorAuditoria;


        // GET: auditorias
        public ActionResult Index(string usuario,
            int p = 1)
        {
        
                int _TotalRegistros = 0;
                int _TotalPaginas = 0;

                _Auditoria = db.auditoria.ToList();

                if (!string.IsNullOrEmpty(usuario))
                {
                    _Auditoria = _Auditoria.Where(x =>
                x.idUsuario.ToLower().Contains(usuario.ToLower())).ToList();
                }

                _TotalRegistros = _Auditoria.Count();
            _Auditoria = _Auditoria.OrderBy(x => x.id)
                            .Skip((p - 1) * _RegistrosPorPagina)
                            .Take(_RegistrosPorPagina)
                            .ToList();

                _TotalPaginas = (int)Math.Ceiling((double)_TotalRegistros / _RegistrosPorPagina);

                auditoriaVar list = new auditoriaVar()
                {
                    idUsuario = usuario,

                };

                _PaginadorAuditoria = new PaginadorAuditoria<auditoria>()
                {
                    RegistrosPorPagina = _RegistrosPorPagina,
                    TotalRegistros = _TotalRegistros,
                    TotalPaginas = _TotalPaginas,
                    PaginaActual = p,
                    Resultado = _Auditoria,
                    tempVar = list
                };

                return View(_PaginadorAuditoria);
            
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
