using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros
{
    /// <summary>
    /// Entidad que realiza el mapping de tabla AdmisionGiros_GIR en Icontroller
    /// </summary>
    public class AdmisionGiros_GIR
    {
        public long NumeroGiro { get; set; }
        public long IdAdmisionGiro { get; set; }
        public string DigitoVerificacion { get; set; }
        public DateTime FechaGrabacion { get; set; }
        public string Estado { get; set; }
        public long ValorGiro { get; set; }
        public long ValorTotal { get; set; }
        public string CreadoPor { get; set; }
        public string IdTipoIdentificacionRemitente { get; set; }
        public string IdRemitente { get; set; }
        public string NombreRemitente { get; set; }
        public string TelefonoRemitente { get; set; }
        public string EmailRemitente { get; set; }
        public long IdCentroServicioOrigen { get; set; }
        public string NombreCentroServicioOrigen { get; set; }
        public string IdTipoIdentificacionDestinatario { get; set; }
        public string IdDestinatario { get; set; }
        public string NombreDestinatario { get; set; }
        public string TelefonoDestinatario { get; set; }
        public string EmailDestinatario { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string NombreCentroServicioDestino { get; set; }
        public string ImagenGiro { get; set; }
    }
}