using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.GirosRest
{
    public class Tercero
    {
        public int TipoIdentificacion { get; set; }
        public string Identificacion { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string RazonSocial { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public DateTime? FechaGrabacion { get; set; }
        public int CreadoPor { get; set; }
        public int IdTercero { get; set; }
        public bool EsEmpleado { get; set; }
        public bool EsCliente { get; set; }
        public bool EsPersonaJuridica { get; set; }
        public string DireccionNormalizada { get; set; }
        public string ClaveTelefonicaGiros { get; set; }
        public bool EsProveedor { get; set; }
        public bool EsContratista { get; set; }
    }
}
