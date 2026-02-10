using System;

namespace ServiciosInter.DatosCompartidos.Wrappers.Preenvios
{
    public class PreenvioAdmisionWrapper
    {
        public long IdPreenvio { get; set; }

        public long NumeroPreenvio { get; set; }

        public long IdRecogida { get; set; }

        public string IdUnidadNegocio { get; set; }

        public int IdServicio { get; set; }

        public string NombreServicio { get; set; }

        public string NombreServicioController { get; set; }

        public string IdTipoEntrega { get; set; }

        public long IdCentroServicioDestino { get; set; }

        public string NombreCentroServicioDestino { get; set; }

        public string DireccionCentroServicioDestino { get; set; }

        public string IdentificacionRepLegal { get; set; }

        public string NombreRepLegal { get; set; }

        public string IdPaisOrigen { get; set; }

        public string IdCiudadOrigen { get; set; }

        public string CiudadOrigen { get; set; }

        public string CodigoPostalOrigen { get; set; }

        public string IdPaisDestino { get; set; }

        public string IdCiudadDestino { get; set; }

        public string CiudadDestino { get; set; }

        public string CodigoPostalDestino { get; set; }

        public string TelefonoDestinatario { get; set; }

        public string DireccionDestinatario { get; set; }

        public string TipoCliente { get; set; }

        public short DiasDeEntrega { get; set; }

        public DateTime FechaEstimadaEntrega { get; set; }

        public decimal ValorAdmision { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorTotalImpuestos { get; set; }

        public decimal ValorTotalRetenciones { get; set; }

        public decimal ValorPrimaSeguro { get; set; }

        public decimal ValorEmpaque { get; set; }

        public decimal ValorAdicionales { get; set; }

        public decimal ValorDeclarado { get; set; }

        public string DiceContener { get; set; }

        public DateTime FechaPreenvio { get; set; }

        public decimal Peso { get; set; }

        public short IdTipoEnvio { get; set; }

        public bool EsAlCobro { get; set; }

        public string IdTipoIdentificacionRemitente { get; set; }

        public long IdRemitente { get; set; }

        public string IdentificacionRemitente { get; set; }

        public string NombreRemitente { get; set; }

        public string ApellidoRemitente { get; set; }

        public string TelefonoRemitente { get; set; }

        public string DireccionRemitente { get; set; }

        public string ComplementoDireccionRemitente { get; set; }

        public string IdTipoIdentificacionDestinatario { get; set; }

        public long IdDestinatario { get; set; }

        public string IdentificacionDestinatario { get; set; }

        public string NombreDestinatario { get; set; }

        public string ApellidoDestinatario { get; set; }

        public long IdEstadoPreenvioLog { get; set; }

        public short IdEstadoPreenvio { get; set; }

        public string DescripcionEstado { get; set; }

        public DateTime FechaGrabacionEstado { get; set; }

        public string ZonaMensajeria { get; set; }

        public string ZonaCarga { get; set; }

        public short NumeroPieza { get; set; }

        public short IdFormaPago { get; set; }

        public string NombreFormaPago { get; set; }

        public DateTime FechaRecogida { get; set; }

        public string DescripcionTipoEntrega { get; set; }

        public string NombreCiudadOrigen { get; set; }
        public string NombreCiudadDestino { get; set; }

        public string Observaciones { get; set; }

        public decimal PesoLiqVolumetrico { get; set; }

        public decimal PesoLiqMasa { get; set; }

        public bool? EsPesoVolumetrico { get; set; }

        public string NumeroBolsaSeguridad { get; set; }

        public short IdMotivoNoUsoBolsaSegurida { get; set; }

        public string MotivoNoUsoBolsaSeguriDesc { get; set; }

        public string NoUsoaBolsaSeguridadObserv { get; set; }

        public string IdUnidadMedida { get; set; }
        public decimal Largo { get; set; }

        public decimal Ancho { get; set; }

        public decimal Alto { get; set; }

        public string NombreTipoEnvio { get; set; }

        public string EmailRemitente { get; set; }

        public string EmailDestinatario { get; set; }

        public long IdCentroServicioOrigen { get; set; }

        public string NombreCentroServicioOrigen { get; set; }

        public int IdCaja { get; set; }

        public long CodigoConvenio { get; set; }

        public long IdSucursal { get; set; }

        public long IdCliente { get; set; }

        public string NombrePaisOrigen { get; set; }

        public string NombrePaisDestino { get; set; }

        public NotificacionWrapper Notificacion { get; set; }

        public RapiradicadoWrapper Rapiradicado { get; set; }
        public DateTime FechaVencimiento { get; set; }

        public long IdPreenvioRecogida { get; set; }

        public string LugarRecogidaEnvioRemi { get; set; }

        public string NombreDestinatarioCompleto { get; set; }

        public string NombreRemitenteCompleto { get; set; }

        public long CodigoConvenioRemitente { get; set; }

        public long IdClienteCredito { get; set; }

        public long IdContrato { get; set; }

        public long IdListaPrecios { get; set; }

        public decimal ValorContrapago { get; set; }

        public string ZonaPostal { get; set; }

        public bool? VerificacionContenido { get; set; }

        public string DirDestiNormalizada { get; set; }

        public string Barrio { get; set; }

        public string LocalidadDestiGeo { get; set; }

        public string EstadoGeoDesti { get; set; }

    }
}
