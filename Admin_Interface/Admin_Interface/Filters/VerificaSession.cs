using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin_Interface.Models.Security;
using Admin_Interface.Controllers;
using System.Net;
using Admin_Interface.Helpers;
using Admin_Interface.Models;

namespace Admin_Interface.Filters
{
    public class VerificaSession : ActionFilterAttribute
    {
        private usuarios oUsuario;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);

                oUsuario = (usuarios)HttpContext.Current.Session["User"];
                if (oUsuario == null)
                {

                    if (filterContext.Controller is AccesoController == false)
                    {
                        filterContext.HttpContext.Response.Redirect("/Acceso/Login");
                    }

                }

            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Acceso/Login");
            }

        }
        //private UserLoged oUsuario;
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    try
        //    {
        //        base.OnActionExecuting(filterContext);
        //        if (HttpContext.Current.Session["User"] == null)
        //        {
        //            filterContext.HttpContext.Response.Redirect("/Acceso/Login");
        //            return;
        //        }
        //        oUsuario = (UserLoged)HttpContext.Current.Session["User"];
        //        var controller = filterContext.RouteData.Values["controller"];
        //        var action = filterContext.RouteData.Values["action"];
        //        bool permiso = true;
        //        foreach (var item in oUsuario.Permisos)
        //        {
        //            if (item.Controller == controller.ToString())
        //                permiso = false;
        //        }
        //        if (permiso)
        //        {
        //            filterContext.HttpContext.Response.Redirect("~/Home/Index");
        //            AuditoriaHelper.Auditoria();
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Helpers.ErroresHelper.AgregarError(ex.Message, "Filters", Helpers.AuditoriaHelper.obtenerIp(Dns.GetHostName()));
        //        filterContext.Result = new RedirectResult("~/Acceso/Login");
        //    }

        //}

    }
}