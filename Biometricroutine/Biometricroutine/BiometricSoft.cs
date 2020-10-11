using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;

namespace Biometricroutine
{
    class BiometricSoft
    {
        ZKSoftware device = new ZKSoftware(Modelo.X628C);

        BIOMETRICOEntities db = new BIOMETRICOEntities();
        
        public bool main(string option, string sucursal, string search)
        {
            bool result = false;
            try
            {
                Console.WriteLine("Conectando a la Base de Datos...");

                Console.WriteLine("¡Conectado!");

                List<biometricos> _Biometricos = null;

                if (string.IsNullOrEmpty(sucursal))
                {
                    Console.Write("Obteniendo Direcciónes IP Activas:... ");

                    _Biometricos = db.biometricos.Where(x =>
                        x.status == 1).ToList();
                }
                else
                {
                    Console.Write("Dirección(es) IP de Sucursal " + sucursal + " a Procesar:... ");

                    _Biometricos = db.biometricos.Where(x =>
                        x.status == 1 && x.id_subdivision == sucursal).ToList();
                }

                if (_Biometricos == null)
                {
                    LogsERROR("Procesando Dirección IP de Biometrico", "No se encontró ningúna dirección IP Válida en la Base de Datos", "");
                    Console.WriteLine("No se encontró ningúna dirección IP Válida en la Base de Datos");
                }
                else
                {
                    Console.WriteLine("¡Obtenido!");

                    Console.WriteLine("--------------------");

                    foreach (var item in _Biometricos)
                    {
                        Console.WriteLine("    " + item.ip);
                    }

                    Console.WriteLine("--------------------");

                    Console.WriteLine("");

                    foreach (biometricos _Bio in _Biometricos)
                    {
                        Console.WriteLine("-------------------------------------------------------------------------------");

                        Console.WriteLine("Conectando a Dispositivo Biometrico: " + _Bio.ip);

                        if (device.DispositivoConectar(_Bio.ip, 0, true))
                        {
                            Console.WriteLine("¡Conectado!");

                            switch (option)
                            {
                                case "GetMarcaciones":
                                    if (CaptureMarcDataToSQL(_Bio))
                                    {
                                        return true;
                                    }
                                    break;
                                case "SetAllUser":
                                    if (RegisterAllUserToDevice(_Bio))
                                    {
                                        return true;
                                    }
                                    break;
                                case "GetAllUser":
                                    if (CaptureUserAllToSQL(_Bio))
                                    {
                                        return true;
                                    }
                                    break;
                                case "DeleteAllUser":
                                    //DeleteAllUserToDevice(_Bio);
                                    break;
                                case "SetUser":
                                    if ((!string.IsNullOrEmpty(_Bio.ip)) &&
                                        (!string.IsNullOrEmpty(search)))
                                    { 
                                        if (RegisterUserToDevice(_Bio.ip, search))
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        LogsERROR("SetUser", "Falta la dirección ip o pernr a buscar", _Bio.ip);
                                    }
                                    break;
                                case "DeleteUser":
                                    if ((!string.IsNullOrEmpty(_Bio.ip)) &&
                                        (!string.IsNullOrEmpty(search)))
                                    {
                                        if (DeleteUserToDevice(_Bio.ip, search))
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        LogsERROR("DeleteUser", "Falta la dirección ip o pernr a buscar", _Bio.ip);
                                    }
                                    
                                    break;
                            }
                        }
                        else
                        {
                            LogsERROR("Conectar Dispositivo", device.ERROR, _Bio.ip);

                            Console.WriteLine(device.ERROR);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        private bool CaptureMarcDataToSQL(biometricos _Bio) // Captura Datos de marcaciones y guarda en Base de Datos. 
        {
            int insert = 0; bool result = false;

            Console.WriteLine("Obteniendo Registros de Marcajes...");

            List<sap_marcaciones> _Sap_marcaciones = db.sap_marcaciones.ToList();

            var cou = _Sap_marcaciones.Count();

            if (device.DispositivoObtenerRegistrosAsistencias())
            {
                List<UsuarioMarcaje> listamarcajes = device.ListaMarcajes;
                Console.WriteLine("Obtenido...");

                int countlista = listamarcajes.Count;
                double porcent = 0; int load = 1;

                Console.Write("Preparando registro de marcaciones... ");

                int y = Console.CursorTop;
                int x = Console.CursorLeft;
                string zatza = "", dallf = "";
                int Dia = 0;

                foreach (var list in listamarcajes)
                {
                    sap_marcaciones lista = new sap_marcaciones();

                    string credencial = list.NumeroCredencial.ToString(), NroC = "";
                    string date = list.Anio.ToString() + times(list.Mes) + times(list.Dia);
                    DateTime fecha = DateTime.Parse(list.Anio.ToString() + "-" + list.Mes.ToString() + "-" + list.Dia.ToString());
                    string time = times(list.Hora) + times(list.Minuto) + times(list.Segundo);
                    DateTime fechabio = DateTime.Parse(list.Anio.ToString() + "-" + list.Mes.ToString() + "-" + list.Dia.ToString()
                        + " " + times(list.Hora) + ":" + times(list.Minuto) + ":" + times(list.Segundo));
                    string fbio = fechabio.ToString("yyyyMMdd HHmmss");

                    switch (list.Zatza)
                    {
                        case 0:
                            zatza = "P10";
                            Dia = list.Dia;
                            NroC = credencial;
                            break;
                        case 1:
                            zatza = "P20";
                            time = getTimeRound(time);
                            break;
                        case 4:
                            zatza = "P15";
                            break;
                        case 5:
                            zatza = "P25";
                            break;
                    }

                    if ((Dia != list.Dia) && (zatza != "P10"))
                        dallf = "X";
                    else
                        dallf = "";

                    lista.pernr = credencial;
                    lista.ldate = date;
                    lista.ltime = time;
                    lista.zatza = zatza;
                    lista.dallf = dallf;
                    lista.status = 0;
                    lista.fulldata = list.NumeroCredencial.ToString() + date + time + zatza + dallf;
                    lista.id_biometrico = _Bio.id;
                    lista.fecha = fecha;
                    lista.date_capture = DateTime.Now;

                    if (cou > 0)
                    {
                        bool existe = false;
                        foreach (var _Sap in _Sap_marcaciones)
                        {
                            if (fbio == (_Sap.ldate + " " + _Sap.ltime.ToString()))
                                existe = true;
                        }
                        if (!existe)
                        {
                            db.sap_marcaciones.Add(lista);
                            insert++;
                        }
                    }
                    else
                    {
                        db.sap_marcaciones.Add(lista);
                        insert++;
                    }
                    porcent = (load * 100) / countlista;

                    Console.SetCursorPosition(x, y);
                    Console.Write("CARGANDO " + porcent + "%");
                    load++;
                }
                Console.SetCursorPosition(1, y + 1);
                if (insert > 0)
                {
                    db.SaveChanges();
                    Console.WriteLine(insert + " Datos Guardados exitosamente.");
                    result = true;

                    Console.WriteLine("Eliminando Historial de Marcaciones en el Dispositivo...");

                    if (device.DispositivoBorrarRegistrosAsistencias())
                    {
                        Console.WriteLine("Historial eliminado correctamente");
                    }
                    else
                    {
                        result = false;
                        LogsERROR("Borrar Marcaciones", device.ERROR, _Bio.ip);
                        Console.WriteLine(device.ERROR);
                    }
                }
                else
                    Console.WriteLine("No existen datos nuevos en el dispositivo");
            }
            else
            {
                LogsERROR("Obtener Asistencias", device.ERROR, _Bio.ip);
                Console.WriteLine(device.ERROR);
            }

            return result;
        } 
        
        private bool RegisterAllUserToDevice(biometricos _Bio) // Registra todos los usuarios a los biometricos 
        {
            double porcent = 0; int errors = 0, load = 1; bool result = false;
            List<sap_empleados> empleados = db.sap_empleados.Where(x =>
                        x.sucursal == _Bio.id_subdivision).ToList();

            int countlista = empleados.Count();

            Console.WriteLine("Registrando Usuarios al Biometrico... ");

            int v = Console.CursorTop;
            int h = Console.CursorLeft;

            foreach (sap_empleados staff in empleados)
            {
                List<finger_empleados> finger = db.finger_empleados.Where(x =>
                            x.pernr == staff.pernr).ToList();

                int fingerError = 0;
                string messaje = "";

                if (!device.UsuarioAgregar(staff.pernr, staff.name + " " + staff.lastname, 
                    Permiso.UsuarioNormal, finger, ref fingerError, ref messaje))
                {
                    errors++;
                    LogsERROR("Agregar Usuario " + staff.pernr + " a Biometrico", device.ERROR, _Bio.ip);
                    result = false;
                }
                else
                {
                    result = true;
                }

                if (fingerError > 0)
                {
                    errors += fingerError;
                    LogsERROR("Agregar Huella de Usuario " + staff.pernr + " a Biometrico", messaje, _Bio.ip);
                }

                porcent = (load * 100) / countlista;

                Console.SetCursorPosition(h, v);
                Console.Write("CARGANDO " + porcent + "%");
                load++;
            }
            
            if (errors > 0)
            {
                Console.WriteLine("El Registro se completo con " + errors + " errores, Verificar en el Logs de Errores del Sistema.");
            }

            return result;
        }

        private bool CaptureUserAllToSQL(biometricos _Bio) // Registra a la base de datos la información de todos los usuarios 
        {
            Console.WriteLine("Obteniendo Registros de Usuarios...");
            double porcent = 0; int load = 1; bool result = false;

            if (device.UsuarioBuscarTodos(true))
            {
                List<UsuarioInformacion> UserInformation = device.ListaUsuarios;

                Console.WriteLine("Obtenido... ");

                Console.WriteLine("Registrando Usuarios a la Base de datos... ");

                int v = Console.CursorTop;
                int h = Console.CursorLeft;

                int countlista = UserInformation.Count();

                foreach (UsuarioInformacion userinfo in UserInformation)
                {
                    int rows = db.sap_empleados.Where(x =>
                            x.pernr == userinfo.NumeroCredencial.ToString()).Count();
                    
                    if (rows == 0)
                    {
                        sap_empleados staff = new sap_empleados();

                        staff.pernr = userinfo.NumeroCredencial.ToString();
                        staff.sucursal = _Bio.id_subdivision;
                        staff.created = DateTime.Now;
                        staff.name = userinfo.Nombre;
                        staff.status = 0;
                        staff.status_empleado = 0;

                        db.sap_empleados.Add(staff);
                    }
                    
                    for (int i = 0; i < userinfo.Huellas.Count; i++)
                    {
                        finger_empleados Finger = db.finger_empleados.Where(x =>
                                x.pernr == userinfo.NumeroCredencial.ToString() &&
                                x.fingerIndex == i).FirstOrDefault();
                        
                        UsuarioHuella huella = userinfo.Huellas.Where(x => x.IndexHuella == i).FirstOrDefault();

                        if (Finger == null)
                        {
                            finger_empleados Femployess = new finger_empleados();
                            Femployess.pernr = userinfo.NumeroCredencial.ToString();
                            Femployess.fingerIndex = huella.IndexHuella;
                            Femployess.flag = huella.Flag;
                            Femployess.fingerData = huella.B64Huella;
                            Femployess.fingerLength = huella.LongitudHuella;

                            db.finger_empleados.Add(Femployess);
                        }
                        else
                        {
                            Finger.fingerIndex = huella.IndexHuella;
                            Finger.flag = huella.Flag;
                            Finger.fingerData = huella.B64Huella;
                            Finger.fingerLength = huella.LongitudHuella;

                            db.Entry(Finger).State = EntityState.Modified;
                        }
                    }
                    
                    porcent = (load * 100) / countlista;

                    Console.SetCursorPosition(h, v);
                    Console.Write("CARGANDO " + porcent + "%");
                    load++;
                }
                try
                {
                    db.SaveChanges();
                    result = true;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        LogsERROR("CaptureUserAllToSQL", "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
                            eve.Entry.Entity.GetType().Name + eve.Entry.State, _Bio.ip);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);

                            LogsERROR("CaptureUserAllToSQL", "- Property: \"{0}\", Error: \"{1}\"" +
                                ve.PropertyName + ve.ErrorMessage, _Bio.ip);
                        }
                    }

                    throw;
                }

                Console.WriteLine("Registros de Usuarios se completó exitosamente.");
            }
            else
            {
                LogsERROR("UsuarioBuscarTodos", device.ERROR, _Bio.ip);
                Console.WriteLine(device.ERROR);
            }
            return result;
        }

        /*private void DeleteAllUserToDevice(biometricos _Bio) // Elimina todos los usuarios del Biometrico 
        {
            List<string> empleados = db.sap_empleados.Select(x => x.pernr).ToList();

            Console.WriteLine("Obteniendo Registros de Usuarios a Eliminar...");

            foreach (string staff in empleados)
            {
                if (device.UsuarioBuscar(staff))
                {
                    if (!device.UsuarioBorrar(staff))
                    {
                        LogsERROR("Eliminar Todos los Usuarios", device.ERROR, _Bio.ip);
                    }
                }
            }

            if (device.UsuarioBuscarTodos(true))
            {
                Console.WriteLine("No se Eliminaron todos los usuarios, Verificar Logs de Errores...");
            }
            else
            {
                Console.WriteLine("Usuarios Eliminados del Biometrico Exitosamente...");
            }
        }*/

        private bool RegisterUserToDevice(string ip, string pernr) // Registra un Usuario al Biometrico 
        {
            bool result = false;

            sap_empleados staff = db.sap_empleados.Where(x =>
                    x.pernr == pernr).FirstOrDefault();

            if (staff != null)
            {
                List<finger_empleados> finger = db.finger_empleados.Where(x =>
                        x.pernr == pernr).ToList();

                int fingererror = 0;
                string errormessage = "";

                Console.WriteLine("Registrando Usuario al Biometrico... ");

                if (!device.UsuarioAgregar(staff.pernr, staff.name + " " + staff.lastname, 
                    Permiso.UsuarioNormal, finger, ref fingererror, ref errormessage))
                {
                    Console.WriteLine(device.ERROR);
                    LogsERROR("RegisterUserToDevice", device.ERROR, ip);
                }
                else
                {
                    result = true;
                }

                if (fingererror > 0)
                {
                    LogsERROR("Agregar Huella de Usuario " + staff.pernr + " a Biometrico", errormessage, ip);
                    Console.WriteLine("El Registro se completo con " + fingererror + " errores, Verificar en el Logs de Errores del Sistema.");
                }
            }
            else
            {
                Console.WriteLine("Usuario no encontrado en la Base de Datos. Por favor verifique");
            }
            return result;
        }

        private bool DeleteUserToDevice(string ip, string pernr) // Elimina un Usuario del Biometrico 
        {
            bool result = false;

            if (device.UsuarioBuscar(pernr))
            {
                if (!device.UsuarioBorrar(pernr))
                {
                    LogsERROR("Eliminar Un Usuario", device.ERROR, ip);
                    Console.WriteLine(device.ERROR);
                }
                else
                {
                    result = true;
                    Console.WriteLine("El Usuario " + pernr + " ha sido eliminado exitosamente");
                }
            }
            return result;
        }
        
        private string times(int nro)
        {
            string nroformat;
            if (nro < 10)
                nroformat = "0" + nro.ToString();
            else
                nroformat = nro.ToString();
            return nroformat;
        }

        public void LogsERROR(string routine, string error, string ip)
        {
            try
            {
                logs_error logs = new logs_error();

                logs.platform = "Console";
                logs.date = DateTime.Now;
                logs.routine = routine;
                logs.errorMessage = error;
                logs.ip = ip;

                db.logs_error.Add(logs);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string path = @"D:\JhonRiv\Desktop\Logs_error.txt";

                if (!File.Exists(path))
                {
                    using (var tw = new StreamWriter(path, true))
                    {
                        tw.WriteLine(ex.Message + ", Fecha/Hora: " + DateTime.Now.ToString("s"));
                    }
                }
                else if (File.Exists(path))
                {
                    using (var tw = new StreamWriter(path, true))
                    {
                        tw.WriteLine("The next line!");
                    }
                }
            }
        }

        private string getTimeRound(string time)
        {
            string round = "";

            int hora = Convert.ToInt32(time.Substring(0, 2));
            int min = Convert.ToInt32(time.Substring(2, 4));

            if (min < 11)
                min = 0;
            else if ((min >= 11) && (min < 26))
                min = 15;
            else if ((min >= 26) && (min < 41))
                min = 30;
            else if ((min >= 41) && (min < 51))
                min = 45;
            else
            {
                min = 0;
                hora++;
            }

            round = hora.ToString() + times(min) + "00";

            return round;
        }
    }
}