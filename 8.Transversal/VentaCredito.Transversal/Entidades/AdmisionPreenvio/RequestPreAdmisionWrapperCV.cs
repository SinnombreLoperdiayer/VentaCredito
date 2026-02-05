using System;

namespace VentaCredito.Transversal.Entidades.AdmisionPreenvio
{
    public class RequestPreAdmisionWrapperCV
    {
        
        public long IdPreenvio { get; set; }
        public long NumeroPreenvio { get; set; }
        public long IdPreenvioRecogida { get; set; }
        public string IdUnidadNegocio { get; set; }
        public int IdServicio { get; set; }
        public string IdTipoEntrega { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string IdPaisOrigen { get; set; }
        public string IdCiudadOrigen { get; set; }        
        public string CodigoPostalOrigen { get; set; }   
        public string IdPaisDestino { get; set; }   
        public string IdCiudadDestino { get; set; }
        public string CodigoPostalDestino { get; set; }
        public string TipoCliente { get; set; }
        public short DiasDeEntrega { get; set; }
        public DateTime? FechaEstimadaEntrega { get; set; }  
        public decimal ValorTotal { get; set; }   
        public decimal ValorDeclarado { get; set; }      
        public string DiceContener { get; set; }
        public decimal Peso { get; set; }
        public short IdTipoEnvio { get; set; }
        public bool? EsAlCobro { get; set; }
        public short NumeroPieza { get; set; }
        public short IdFormaPago { get; set; }
        public string NombreFormaPago { get; set; }
        public string NombreServicio { get; set; }
        public string DescripcionTipoEntrega { get; set; }
        public string NombreCiudadOrigen { get; set; }
        public string NombreCiudadDestino { get; set; }
        public decimal ValorAdmision { get; set; }
        public decimal ValorTotalImpuestos { get; set; }
        public decimal ValorTotalRetenciones { get; set; }
        public decimal ValorPrimaSeguro { get; set; }
        public decimal ValorEmpaque { get; set; }
        public decimal ValorAdicionales { get; set; }
        public string Observaciones { get; set; }
        public decimal PesoLiqVolumetrico { get; set; }
        public decimal PesoLiqMasa { get; set; }
        public bool EsPesoVolumetrico { get; set; }
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
        public RemitenteVC Remitente { get; set; }
        public DestinatarioVC Destinatario { get; set; }
        public NotificacionPE Notificacion { get; set; }
        public RapiradicadoPE Rapiradicado { get; set; }
        public string LugarRecogidaEnvioRemi { get; set; }
        public long CodigoConvenioRemitente { get; set; }
        public long IdClienteCredito { get; set; }
        public int IdListaPrecios { get; set; }
        public int IdContrato { get; set; }
        public bool AplicaContrapago { get; set; }
        public decimal ValorContrapago { get; set; }
        public string Zona1 { get; set; }
        public string Zona2 { get; set; }
        public string Zona3 { get; set; }
        public string ZonaPostal { get; set; }
        public string Barrio { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public decimal ValorACobrar { get; set; }
        public bool RequiereEmpaque { get; set; }
        public string DirDestiNormalizada { get; set; }
        public string EstadoGeoDesti { get; set; }
        public string LocalidadDestiGeo { get; set; }
        public bool? EsMarketplace { get; set; }
    }
}
