using Admin_Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Admin_Interface.Helpers
{

    public static class AuditoriaHelper
    {
        static string mensajeCrear = "Los datos agregados son: {0}";
        static string mensajeEditar = "Los datos Anteriores son: {0} y se modificaron por: {1}";
        static string mensajeEliminar = "Los datos eliminados son: {0}";
        static string mensajeLogear = "El usuario: {0} ,ingreso al sistema desde la ip: {1} a las {2}";
        static string mensajeLogOut = "El usuario: {0} ,salio del sistema desde la ip: {1} a las {2}";
        static string mensajeRegistrar = "El usuario: {0} ,se registro en el sistema desde la ip: {1} a las {2}";

        /// <summary>
        /// Metodo para la uditoria
        /// </summary>
        /// <param name="idUsuario">Id del usuario que esta realizando la accion</param>
        /// <param name="ip">Ip desde de donde se esta ejecutando la accion</param>
        /// <param name="accion">Accion que se esta ejecutando </param>
        /// <param name="proceso">Procese que se esta ejecutando</param>
        /// <param name="description">Descripcion de lo que sucede</param>
        /// <param name="datoAnterior">Opcional: Dato que que existia en Bd y que se esta modificando o eliminado</param>
        /// <param name="datoNuevo">Opcional: Dato ingresado a la base de datos</param>
        public static void Auditoria(string idUsuario,string ip,TipoAccion accion,string proceso,string description,string datoAnterior=null,string datoNuevo=null)
        {
            try
            {
                using (BIOMETRICOEntities dc = new BIOMETRICOEntities())
                {
                    var v = dc.usuarios.Where(a => a.pernr == idUsuario).FirstOrDefault();
                    var oUser = v;
                    string userId = v.pernr;
                    auditoria audit = new auditoria();
                    audit.fechaModificacion = DateTime.Now;
                    audit.ip = ip;
                    audit.idUsuario = userId.ToString();
                    audit.accion = (int)accion;
                    audit.proceso = proceso;
                    audit.idUsuario=idUsuario;
                    audit.description = description;
                    dc.auditoria.Add(audit);
                    dc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErroresHelper.AgregarError(ex.Message, "Auditoria", obtenerIp(Dns.GetHostName()));
            }

        }
        public static string obtenerIp(string hostName)
        {
            
            string localIP = string.Empty;
            IPHostEntry host = Dns.GetHostEntry(hostName);// objeto para guardar la ip
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();// esta es nuestra ip
                }
            }
            return localIP;
        }
        public static string ArmarDescription(TipoAccion tipo,string name)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "nombre: {0}";
            switch (tipo)
            {
                case TipoAccion.Login:
                    cadena = string.Format(formato, name);
                    valor += string.Format(mensajeLogear, cadena,obtenerIp(Dns.GetHostName()),DateTime.Now);
                    break;
                case TipoAccion.Create:
                    break;
                case TipoAccion.Edit:
                    break;
                case TipoAccion.Delete:
                    break;
                case TipoAccion.logOut:
                    cadena = string.Format(formato, name);
                    valor += string.Format(mensajeLogOut, cadena, obtenerIp(Dns.GetHostName()), DateTime.Now);
                    break;
                case TipoAccion.Register:
                    cadena = string.Format(formato, name);
                    valor += string.Format(mensajeRegistrar, cadena, obtenerIp(Dns.GetHostName()), DateTime.Now);
                    break;              
                default:
                    break;
            }
            return valor;
        }

        public static string ArmarDescription(TipoAccion tipo, string name, usuarios datoAnterior, usuarios DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "nombre: {0} {1} Email: {2}";
            switch (tipo)
            {
                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.name, DatoNuevo.lastname, DatoNuevo.email);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    cadena = string.Format(formato, DatoNuevo.name, DatoNuevo.lastname, DatoNuevo.email);
                    string anterior = string.Format(formato, datoAnterior.name, datoAnterior.lastname, datoAnterior.email);
                    valor += string.Format(mensajeEditar, anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.name, datoAnterior.lastname, datoAnterior.email);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;
                case TipoAccion.Register:
                    cadena = string.Format(formato, datoAnterior.name, datoAnterior.lastname, datoAnterior.email);
                    valor += string.Format(mensajeRegistrar, cadena, obtenerIp(Dns.GetHostName()), DateTime.Now);
                    break;
                default:
                    break;
            }
            return valor;
        }

        public static string ArmarDescription(TipoAccion tipo, string name, biometricos datoAnterior, biometricos DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "Modelo: {0} Status: {1} subdivision: {2}";
            switch (tipo)
            {
                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.modelo, DatoNuevo.status, DatoNuevo.id_subdivision);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    cadena = string.Format(formato, DatoNuevo.modelo, DatoNuevo.status, DatoNuevo.id_subdivision);
                    string anterior = string.Format(formato, datoAnterior.modelo, datoAnterior.status, datoAnterior.id_subdivision);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.modelo, datoAnterior.status, datoAnterior.id_subdivision);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;                
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, divisiones datoAnterior, divisiones DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "Nombre: {0} Sociedad: {1} ";
            switch (tipo)
            {

                case TipoAccion.Create:                   
                    cadena = string.Format(formato, DatoNuevo.nombre, DatoNuevo.id_sociedad);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    cadena = string.Format(formato, DatoNuevo.nombre, DatoNuevo.id_sociedad);
                    string anterior= string.Format(formato, datoAnterior.nombre, datoAnterior.id_sociedad);
                    valor += string.Format(mensajeEditar,anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.nombre, datoAnterior.id_sociedad);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;                
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, roles datoAnterior, roles DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "Nivel rol: {0} {1}";
            switch (tipo)
            {

                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.nivel_rol, DatoNuevo.nivel_rol);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    cadena = string.Format(formato, DatoNuevo.nivel_rol, DatoNuevo.nivel_rol);
                    string anterior= string.Format(formato, datoAnterior.nivel_rol, datoAnterior.nivel_rol);
                    valor += string.Format(mensajeEditar,anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.nivel_rol, datoAnterior.nivel_rol);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;              
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, sap_empleados datoAnterior, sap_empleados DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "DNI {0} Nombre: {1} {2} Subdivisión: {3} Sucursal {4}";
            switch (tipo)
            {

                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.dni, DatoNuevo.name, DatoNuevo.lastname, DatoNuevo.sucursal, DatoNuevo.sucursal);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    string anterior = string.Format(formato, datoAnterior.dni, datoAnterior.name, datoAnterior.lastname, datoAnterior.sucursal, datoAnterior.sucursal);
                    cadena = string.Format(formato, DatoNuevo.dni, DatoNuevo.name, DatoNuevo.lastname, DatoNuevo.sucursal, DatoNuevo.sucursal);
                    valor += string.Format(mensajeEditar, anterior,cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.dni, datoAnterior.name, datoAnterior.lastname, datoAnterior.sucursal, datoAnterior.sucursal);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;
                case TipoAccion.Register:
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, sap_marcaciones datoAnterior, sap_marcaciones DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "id Biometrico {0} ldate: {1} ltime: {2} perm: {3} zatza {4}";
            switch (tipo)
            {

                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.id_biometrico, DatoNuevo.ldate, DatoNuevo.ltime, DatoNuevo.pernr, DatoNuevo.zatza);
                    valor += string.Format(mensajeCrear, cadena);
                    break;
                case TipoAccion.Edit:
                    string anterior = string.Format(formato, datoAnterior.id_biometrico, datoAnterior.ldate, datoAnterior.ltime, datoAnterior.pernr, datoAnterior.zatza);
                    cadena = string.Format(formato, DatoNuevo.id_biometrico, DatoNuevo.ldate, DatoNuevo.ltime, DatoNuevo.pernr, DatoNuevo.zatza);
                    valor += string.Format(mensajeEditar, anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.id_biometrico, datoAnterior.ldate, datoAnterior.ltime, datoAnterior.pernr, datoAnterior.zatza);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;
                case TipoAccion.Register:
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, sociedades datoAnterior, sociedades DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "Nombre: {0} ";
            switch (tipo)
            {

                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.nombre);
                    valor += string.Format(mensajeCrear,  cadena);
                    break;
                case TipoAccion.Edit:
                    string anterior = string.Format(formato, datoAnterior.nombre);
                    cadena = string.Format(formato, DatoNuevo.nombre);
                    valor += string.Format(mensajeEditar, anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.nombre);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;               
                default:
                    break;
            }
            return valor;
        }
        public static string ArmarDescription(TipoAccion tipo, string name, subdivisiones datoAnterior, subdivisiones DatoNuevo)
        {
            string valor = string.Empty;
            string cadena;
            string formato = "Nombre: {0} id Subdivision:{1}";
            switch (tipo)
            {
                case TipoAccion.Create:
                    cadena = string.Format(formato, DatoNuevo.nombre, DatoNuevo.id, DatoNuevo.id_division);
                    valor += string.Format(mensajeCrear,  cadena);
                    break;
                case TipoAccion.Edit:
                    string anterior = string.Format(formato, datoAnterior.nombre,datoAnterior.id_division);
                    cadena = string.Format(formato, DatoNuevo.nombre, datoAnterior.id_division);
                    valor += string.Format(mensajeEditar, anterior, cadena);
                    break;
                case TipoAccion.Delete:
                    cadena = string.Format(formato, datoAnterior.nombre, datoAnterior.id_division);
                    valor += string.Format(mensajeEliminar, cadena);
                    break;
                case TipoAccion.Register:
                default:
                    break;
            }
            return valor;
        }


    }
}