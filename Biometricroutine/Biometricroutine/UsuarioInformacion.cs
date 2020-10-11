using System.Collections.Generic;

namespace Biometricroutine
{
    public class UsuarioInformacion
    {
        public string NumeroCredencial { get; set; }
        public string Nombre { get; set; }
        public Permiso Permiso { get; set; }
        public string Contrasenia { get; set; }
        public bool Activo { get; set; }
        public List<UsuarioHuella> Huellas { get; set; }
        public override string ToString() => this.NumeroCredencial.ToString() + (this.Nombre == string.Empty ? string.Empty : " - " + this.Nombre);
    }
}
