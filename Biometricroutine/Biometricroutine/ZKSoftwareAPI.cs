using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using zkemkeeper;

namespace Biometricroutine
{
    public class ZKSoftware
    {
        public CZKEMClass i = new CZKEMClass();

        //private string a;       // Se usa para mostrar mensajes de los metodos
        private int be;
        private List<UsuarioInformacion> c;
        private UsuarioInformacion d;
        private List<UsuarioMarcaje> e;
        private UsuarioInformacion f;
        private bool g;         // Retorna los procesos true o false
        private int h;          // Se almacena los codigos de error
        //private CZKEMClass i;   // Clase de la dll
        private Modelo j;
        private string k = string.Empty; // se usa para almacenar la IP o Vacio
        private int l;      // Se usa en conjunto con m para validar el envio de ping
        private int m;      // Se usa en conjunto con l para validar el envio de ping
        private string messaje;


        public ZKSoftware(Modelo device)
        {
            j = device;
        }

        ~ZKSoftware()
        {
            i = (CZKEMClass) null;
        }

        public string ERROR => messaje;

        public int ResultadoConsulta => this.be;

        public List<UsuarioInformacion> ListaUsuarios => this.c;

        public UsuarioInformacion Usuario => this.d;

        public List<UsuarioMarcaje> ListaMarcajes => this.e;

        private bool b()
        {
            this.l = 0;
            this.m = 0;
            Ping ping = new Ping();
            if (string.IsNullOrEmpty(this.k))
            {
                messaje = "Es necesario ejecutar DispositivoConectar() antes de ejecutar este método.";
                return false;
            }
            for (int index = 0; index < 3; ++index)
            {
                if (ping.Send(this.k).Status == IPStatus.Success)
                    ++this.l;
                else
                    ++this.m;
            }
            if (this.m <= this.l)
                return true;
            messaje = "No se encontró el dispositivo.";
            return false;
        }

        private string a(int A_0)
        {
            messaje = string.Empty;
            switch (A_0)
            {
                case -100:
                    messaje = "Operación fallida o el dato no existe.";
                    break;
                case -10:
                    messaje = "La longitud del dato transmitido no es correcto.";
                    break;
                case -7:
                    messaje = "No se encontró conexión con el dispositivo.";
                    break;
                case -5:
                    messaje = "El dato ya existe.";
                    break;
                case -4:
                    messaje = "El espacio no es suficiente.";
                    break;
                case -3:
                    messaje = "Error de tamaño.";
                    break;
                case -2:
                    messaje = "Error en el archivo (read/write).";
                    break;
                case -1:
                    messaje = "El SDK no está inicializado.";
                    break;
                case 0:
                    messaje = "El dato no se encuentra o está repetido.";
                    break;
                case 1:
                    messaje = "Operación correcta.";
                    break;
                case 4:
                    messaje = "Parámetro incorrecto.";
                    break;
                case 101:
                    messaje = "Error en la asignación del bufer.";
                    break;
            }
            return messaje;
        }

        public bool Beep()
        {
            messaje = string.Empty;
            this.g = false;
            if (this.i.Beep(100))
            {
                this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: Beep) - Error al enviar Beep. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoConectar(string IP, int IntentosConexion, bool bBeep)
        {
            this.k = IP;
            if (!this.b())
                return false;
            if (IntentosConexion == 0)
                IntentosConexion = 1;
            int num = 1;
            messaje = string.Empty;
            this.g = false;
            this.i = new CZKEMClass();
            while (num <= IntentosConexion)
            {
                if (this.i.Connect_Net(IP, 4370))
                {
                    this.g = true;
                    if (bBeep)
                    {
                        Thread.Sleep(300);
                        this.Beep();
                        Thread.Sleep(300);
                        this.Beep();
                        Thread.Sleep(300);
                        this.Beep();
                    }
                    num = IntentosConexion + 1;
                    this.k = IP;
                }
                else
                {
                    this.k = string.Empty;
                    ++num;
                    this.i.GetLastError(ref this.h);
                    messaje = "(Método: Conectar) - Error al conectar el dispositivo. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    this.DispositivoDesconectar();
                    this.i = new CZKEMClass();
                }
            }
            return this.g;
        }

        public void DispositivoDesconectar()
        {
            if (this.i == null)
                return;
            this.i.Disconnect();
        }

        public bool DispositivoConsultar(NumeroDe DatoConsultar)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            this.be = 0;
            if (this.i.GetDeviceStatus(1, (int)DatoConsultar, ref this.be))
            {
                this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: Consultar) - Error al Consultar: " + DatoConsultar.ToString() + ". Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoCambiarEstatus(Estatus Estatus)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (this.i.EnableDevice(1, Estatus != Estatus.Deshabilitar))
            {
                this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: CambiarEstatus) - Error al " + Estatus.ToString() + " el dispositivo.  Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoObtenerRegistrosAsistencias()
        {
            try
            {
                if (!this.b())
                    return false;
                messaje = string.Empty;
                this.g = false;
                this.e = new List<UsuarioMarcaje>();
                if (!this.i.ReadGeneralLogData(1))
                {
                    this.i.GetLastError(ref this.h);
                    messaje = "(Método: DispositivoObtenerRegistrosAsistencias) - Error al leer el log de marcajes. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    return this.g;
                }
                string dwEnrollNumber = string.Empty;
                int dwVerifyMode = 0;
                int dwInOutMode = 0;
                int dwYear = 0;
                int dwMonth = 0;
                int dwDay = 0;
                int dwHour = 0;
                int dwMinute = 0;
                int dwSecond = 0;
                int dwWorkCode = 0;
                while (this.i.SSR_GetGeneralLogData(1, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                    this.e.Add(new UsuarioMarcaje()
                    {
                        NumeroCredencial = int.Parse(dwEnrollNumber),
                        Zatza = dwInOutMode,
                        Anio = dwYear,
                        Mes = dwMonth,
                        Dia = dwDay,
                        Hora = dwHour,
                        Minuto = dwMinute,
                        Segundo = dwSecond
                    });
                this.g = true;
            }
            catch (Exception ex)
            {
                messaje = "(Método: DispositivoObtenerRegistrosAsistencias) - Error al leer el log de marcajes. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoBorrarRegistrosAsistencias()
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (this.i.ClearGLog(1))
            {
                this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: DispositivoBorrarRegistrosAsistencias) - Error al borrar los registros de asistencias. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoObtenerRegistrosOperativos()
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            this.e = new List<UsuarioMarcaje>();
            if (!this.i.ReadSuperLogData(1))
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: DispositivoObtenerRegistrosOperativos) - Error al leer el log de marcajes operativos. Código de error: " + this.h.ToString() + " - " + a(this.h);
                return this.g;
            }
            int dwSEnrollNumber = 0;
            int dwTMachineNumber = 0;
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwManipulation = 0;
            int Params1 = 0;
            int Params2 = 0;
            int Params3 = 0;
            int Params4 = 0;
            while (this.i.GetSuperLogData(1, ref dwTMachineNumber, ref dwSEnrollNumber, ref Params4, ref Params1, ref Params2, ref dwManipulation, ref Params3, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute))
            {
                UsuarioMarcaje usuarioMarcaje = new UsuarioMarcaje()
                {
                    NumeroCredencial = dwSEnrollNumber,
                    Anio = dwYear,
                    Mes = dwMonth,
                    Dia = dwDay,
                    Hora = dwHour,
                    Minuto = dwMinute,
                    MarcajeOperativo = new MarcajeOperativo()
                };
                usuarioMarcaje.MarcajeOperativo.NumeroDispositivo = dwTMachineNumber;
                usuarioMarcaje.MarcajeOperativo.Operacion = dwManipulation;
                usuarioMarcaje.MarcajeOperativo.Parametro1 = Params1;
                usuarioMarcaje.MarcajeOperativo.Parametro2 = Params2;
                usuarioMarcaje.MarcajeOperativo.Parametro3 = Params3;
                usuarioMarcaje.MarcajeOperativo.Parametro4 = Params4;
                this.e.Add(usuarioMarcaje);
            }
            this.g = true;
            return this.g;
        }

        public bool DispositivoBorrarRegistrosOperativos()
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (this.i.ClearSLog(1))
            {
                this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: DispositivoBorrarRegistrosOperativos) - Error al eliminar los registros operativos. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool DispositivoCambiarHoraAutomatico()
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (!this.DispositivoCambiarEstatus(Estatus.Deshabilitar))
                return this.g;
            if (this.i.SetDeviceTime(1))
            {
                if (this.DispositivoCambiarEstatus(Estatus.Habilitar))
                    this.g = true;
            }
            else
                messaje = "(Método: DispositivoCambiarHoraAutomatico) - Error al cambiar la hora del dispositivo. Código de error: " + this.h.ToString() + " - " + a(this.h);
            return this.g;
        }

        public bool DispositivoCambiarHoraManual(
          int iDia,
          int iMes,
          int iAnio,
          int iHora,
          int iMinuto,
          int iSegundo)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (!this.DispositivoCambiarEstatus(Estatus.Deshabilitar))
                return this.g;
            if (this.i.SetDeviceTime2(1, iAnio, iMes, iDia, iHora, iMinuto, iSegundo))
            {
                if (this.DispositivoCambiarEstatus(Estatus.Habilitar))
                    this.g = true;
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: DispositivoCambiarHoraManual) - Error al cambiar la hora del dispositivo de forma manual. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        private bool a()
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (!this.i.SendFile(1, "C:\\Users\\Gabriel\\Desktop\\ad_1.jpg"))
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: DispositivoEnviarImagen) - Error al eliminar los registros operativos. Código de error: " + this.h.ToString() + " - " + a(this.h);
            }
            return this.g;
        }

        public bool UsuarioAgregar(
          string NumeroCredencial,
          string UsuarioNombre,
          Permiso TipoPermiso,
          List<finger_empleados> finger,
          ref int fingerError,
          ref string fingerErrorMessaje)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (this.i.SSR_SetUserInfo(1, NumeroCredencial, UsuarioNombre, string.Empty, (int)TipoPermiso, true))
            {
                if (finger.Count > 0)
                {
                    foreach (finger_empleados Fstaff in finger)
                    {
                        if (this.i.SetUserTmpExStr(1, NumeroCredencial, Fstaff.fingerIndex, Fstaff.flag, Fstaff.fingerData))
                        {
                            this.g = true;
                        }
                        else
                        {
                            this.i.GetLastError(ref this.h);
                            fingerError++;
                            messaje = "(Método: UsuarioEnrolar) - Error al guardar la huella. Código de error: " + this.h.ToString() + " - " + a(this.h);
                        }
                    }
                }
            }
            else
            {
                this.i.GetLastError(ref this.h);
                messaje = "(Método: UsuarioEnrolar) - Error al enrolar el usuario. Código de error: " + this.h.ToString() + " - " + a(this.h);
                fingerErrorMessaje = messaje;
            }
            return this.g;
        }

        public bool UsuarioBorrar(string NumeroCredencial)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (this.i.SSR_DeleteEnrollDataExt(1, NumeroCredencial, 12))
                this.g = true;
            else
                messaje = "(Método: UsuarioBorrar) - Error al eliminar el usuario. Código de error: " + this.h.ToString() + " - " + a(this.h);
            return this.g;
        }

        public bool UsuarioBuscarTodos(bool DeshabilitarDispositivo)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            if (DeshabilitarDispositivo && !this.DispositivoCambiarEstatus(Estatus.Deshabilitar))
            {
                messaje = "(Método: UsuarioBuscarTodos) - Error al cambiar estatus de dispositivo. Código de error: " + this.h.ToString() + " - " + a(this.h);
                return this.g;
            }
            if (this.i.ReadAllUserID(1))
            {
                if (!this.i.ReadAllTemplate(1))
                {
                    messaje = "(Método: UsuarioBuscarTodos) - Error al obtener las huellas de los usuarios. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    return this.g;
                }
                string dwEnrollNumber = string.Empty;
                string Name = string.Empty;
                string Password = string.Empty;
                int Privilege = 0;
                bool Enabled = false;
                string TmpData = string.Empty;
                int TmpLength = 0;
                int Flag = 0;
                if (!this.i.ReadAllUserID(1))
                {
                    messaje = "(Método: UsuarioBuscarTodos) - No se pudo obtener la información de la memoria. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    return this.g;
                }
                if (!this.i.ReadAllTemplate(1))
                {
                    messaje = "(Método: UsuarioBuscarTodos) - No se pudieron obtener las huellas de la memoria. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    return this.g;
                }
                this.c = new List<UsuarioInformacion>();
                while (this.i.SSR_GetAllUserInfo(1, out dwEnrollNumber, out Name, out Password, out Privilege, out Enabled))
                {
                    this.f = new UsuarioInformacion();
                    this.f.NumeroCredencial = dwEnrollNumber;
                    this.f.Nombre = Name;
                    this.f.Permiso = (Permiso)Privilege;
                    this.f.Contrasenia = Password;
                    this.f.Activo = Enabled;
                    this.f.Huellas = new List<UsuarioHuella>();
                    for (int dwFingerIndex = 0; dwFingerIndex < 10; ++dwFingerIndex)
                    {
                        if (this.i.GetUserTmpExStr(1, dwEnrollNumber, dwFingerIndex, out Flag, out TmpData, out TmpLength))
                            this.f.Huellas.Add(new UsuarioHuella()
                            {
                                IndexHuella = dwFingerIndex,
                                Flag = Flag,
                                B64Huella = TmpData,
                                LongitudHuella = TmpLength
                            });
                    }
                    this.c.Add(this.f);
                }
                if (DeshabilitarDispositivo && !this.DispositivoCambiarEstatus(Estatus.Habilitar))
                {
                    messaje = "(Método: UsuarioBuscarTodos) - Error al habilitar el dispositivo. Código de error: " + this.h.ToString() + " - " + a(this.h);
                    return this.g;
                }
                this.g = true;
                return this.g;
            }
            messaje = "(Método: UsuarioBuscarTodos) - Error al obtener la información de los usuarios. Código de error: " + this.h.ToString() + " - " + a(this.h);
            return this.g;
        }

        public bool UsuarioBuscar(string NumeroCredencial)
        {
            if (!this.b())
                return false;
            messaje = string.Empty;
            this.g = false;
            string Name = string.Empty;
            string Password = string.Empty;
            string TmpData = string.Empty;
            int Privilege = 0;
            int TmpLength = 0;
            bool Enabled = false;
            if (this.i.SSR_GetUserInfo(1, NumeroCredencial, out Name, out Password, out Privilege, out Enabled))
            {
                this.d = new UsuarioInformacion();
                this.d.NumeroCredencial = NumeroCredencial;
                this.d.Nombre = Name;
                this.d.Contrasenia = Password;
                this.d.Permiso = (Permiso)Privilege;
                this.d.Activo = Enabled;
                this.d.Huellas = new List<UsuarioHuella>();
                for (int dwFingerIndex = 0; dwFingerIndex < 10; ++dwFingerIndex)
                {
                    if (this.i.SSR_GetUserTmpStr(1, NumeroCredencial.ToString(), dwFingerIndex, out TmpData, out TmpLength))
                        this.d.Huellas.Add(new UsuarioHuella()
                        {
                            IndexHuella = dwFingerIndex,
                            B64Huella = TmpData,
                            LongitudHuella = TmpLength
                        });
                }
                this.g = true;
            }
            else
                messaje = "(Método: UsuarioBuscar) - Error al buscar el usuario. Código de error: " + this.h.ToString() + " - " + a(this.h);
            return this.g;
        }
    }
}

