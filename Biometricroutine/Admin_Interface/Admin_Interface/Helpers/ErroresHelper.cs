using Admin_Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin_Interface.Helpers
{
    /// <summary>
    /// Clase que se encarga de guardar todos los errores en la Base de datos 
    /// en la tabla correspondiente. 
    /// </summary>
    public static class ErroresHelper
    {
        /// <summary>
        /// Metodo que se encarga de Procesar la solicitud de registro de un error.
        /// </summary>
        /// <param name="exeption"></param>
        /// <param name="Debes ingresar el mensaje del error"></param>
        /// <param name="routine"></param>
        /// <param name="Debes ingresar la rutina del error"></param>
        /// /// <param name="ip"></param>
        /// <param name="Debes ingresar la ip del usuario que generó el error"></param>
        public static void AgregarError(string exeption,string routine,string ip)
        {
            using (BIOMETRICOEntities dc = new BIOMETRICOEntities())
            {
                logs_error logs = new logs_error();
                logs.date = DateTime.Now;
                logs.errorMessage = exeption;
                logs.routine = routine;
                logs.ip = ip;
                logs.platform = "Página Administrativa";
                dc.logs_error.Add(logs);
                dc.SaveChanges();
            }
        }
    }
}