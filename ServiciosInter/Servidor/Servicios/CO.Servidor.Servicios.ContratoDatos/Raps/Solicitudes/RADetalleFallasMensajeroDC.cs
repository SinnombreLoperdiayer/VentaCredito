using System;
using System.Collections.Generic;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public class RAEstadoActualSolicitudRapsDC
    {
        public string IdMensajero { get; set; }
        public string NombreMensajero { get; set; }
        public string TelefonoMensajero { get; set; }
        public string SucursalMensajero { get; set; }
        public string FotoMensajero { get; set; }
        public string ProcesoResponsable { get; set; }
        public string IdentificacionResponsable { get; set; }
        public string NombreResponsable { get; set; }
        public string TelefonoResponsable { get; set; }
        public string FechaVencimiento { get; set; }
        public string FotoResponsable { get; set; }
    }
    public class RAFallasMensajeroDC
    {
        public int IdParametrizacion { get; set; }
        public string Descripcion { get; set; }
        public int IdSolicitud { get; set; }
        public int Cuenta { get; set; }
        public int NivelGravedad { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Anchor { get; set; }
    }
    public class RADetalleFallasMensajeroDC
    {
        public RAEstadoActualSolicitudRapsDC Encabezado { get; set; }
        public List<RAFallasMensajeroDC> FallasMensajero { get; set; }
        public List<RAAdjuntoDC> AdjuntosGestion { get; set; }
        public string ComentarioGestion { get; set; }
    }
}
