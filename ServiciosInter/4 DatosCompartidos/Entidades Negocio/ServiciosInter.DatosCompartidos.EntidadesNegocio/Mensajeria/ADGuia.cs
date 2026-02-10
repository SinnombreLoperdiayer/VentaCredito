using System;
using System.Collections.Generic;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADGuia
    {
        public long IdTrazaGuia { get; set; }

        public CLClienteContadoDC Remitente { get; set; }

        public CLClienteContadoDC Destinatario { get; set; }

        public long IdAdmision { get; set; }

        public long NumeroGuia { get; set; }

        public long NumeroGuiaDHL { get; set; }

        
        public string PrefijoNumeroGuia { get; set; }

        public string DigitoVerificacion { get; set; }

        public string GuidDeChequeo { get; set; }

        public bool EsAutomatico { get; set; }

        public string TipoTarifa { get { return EsAutomatico == true ? "Automático" : "Manual"; } }

        public string IdUnidadNegocio { get; set; }

        public int IdServicio { get; set; }

        public string NombreServicio { get; set; }

        public string IdTipoEntrega { get; set; }

        public string DescripcionTipoEntrega { get; set; }

        public long IdCentroServicioOrigen { get; set; }

        public string NombreCentroServicioOrigen { get; set; }

        public long IdCentroServicioDestino { get; set; }

        public string NombreCentroServicioDestino { get; set; }

        public string IdPaisOrigen { get; set; }

        public string NombrePaisOrigen { get; set; }

        public string IdCiudadOrigen { get; set; }

        public string NombreCiudadOrigen { get; set; }

        public string CodigoPostalOrigen { get; set; }

        public string IdPaisDestino { get; set; }

        public string NombrePaisDestino { get; set; }

        public string IdCiudadDestino { get; set; }

        public string NombreCiudadDestino { get; set; }

        public string CodigoPostalDestino { get; set; }

        public string TelefonoDestinatario { get; set; }

        public string DireccionDestinatario { get; set; }

        public ADEnumTipoCliente TipoCliente { get; set; }

        public string TipoClienteStr { get; set; }

        public short DiasDeEntrega { get; set; }

        public DateTime FechaEstimadaEntrega { get; set; }

        public DateTime FechaEstimadaEntregaNew { get; set; }

        public DateTime FechaEstimadaDigitalizacion { get; set; }

        public DateTime FechaEstimadaArchivo { get; set; }

        public decimal ValorAdmision { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal ValorTotalImpuestos { get; set; }

        public decimal ValorTotalRetenciones { get; set; }

        public decimal ValorPrimaSeguro { get; set; }

        public decimal ValorEmpaque { get; set; }

        public decimal ValorAdicionales { get; set; }

        public decimal ValorDeclarado { get; set; }

        public string DiceContener { get; set; }

        public string Observaciones { get; set; }

        public short NumeroPieza { get; set; }

        public short TotalPiezas { get; set; }

        public DateTime FechaAdmision { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoLiqVolumetrico { get; set; }

        public decimal PesoLiqMasa { get; set; }

        public bool EsPesoVolumetrico { get; set; }

        public string NumeroBolsaSeguridad { get; set; }

        public short? IdMotivoNoUsoBolsaSegurida { get; set; }

        public string MotivoNoUsoBolsaSeguriDesc { get; set; }

        public string NoUsoaBolsaSeguridadObserv { get; set; }

        public string IdUnidadMedida { get; set; }

        public decimal Largo { get; set; }

        public decimal Ancho { get; set; }

        public decimal Alto { get; set; }

        public string NoPedido { get; set; }

        public bool EsRecomendado { get; set; }

        public short IdTipoEnvio { get; set; }

        public string NombreTipoEnvio { get; set; }

        public bool AdmisionSistemaMensajero { get; set; }

        /// <summary>
        /// Con base en las formas de pago seleccionadas se carga esta bandera en el servidor
        /// </summary>

        public bool EsAlCobro { get; set; }

        public DateTime FechaGrabacion { get; set; }

        public string CreadoPor { get; set; }

        public string ObservacionEstadoGuia { get; set; }

        public ADEnumEstadoGuia EstadoGuia { get; set; }

        public List<ADGuiaFormaPago> FormasPago { get; set; }


        /// <summary>
        /// Contiene el id del concepto de caja asociado al servicio
        /// </summary>

        public int IdConceptoCaja { get; set; }

        /// <summary>
        /// Valor del servicio o valor de la tarifa
        /// </summary>
        public decimal ValorServicio { get; set; }

        /// <summary>
        /// Indica si al momento de grabar la transacción la guía fué pagada
        /// </summary>
        public bool EstaPagada { get; set; }

        /// <summary>
        /// Fecha en la que se hace el pago de la guía
        /// </summary>
        public DateTime? FechaPago { get; set; }

        /// <summary>
        /// Caja de carton en la que se almacena la guia archivada. OJO:Esta no es la caja del punto
        /// </summary>
        public long Caja { get; set; }

        public int Lote { get; set; }

        public int Posicion { get; set; }

        public bool Aprobada { get; set; }

        public bool NoAprobada { get; set; }

        public string MotivoEntrega { get; set; }

        public string FormasPagoDescripcion { get; set; }

        public string FormasPagoIds { get; set; }

        /// <summary>
        /// Es el id del usuario. Rafram 14-09-2012
        /// </summary>
        public long IdCodigoUsuario { get; set; }

        public bool Supervision { get; set; }

        public DateTime FechaSupervision { get; set; }

        public bool NotificarEntregaPorEmail { get; set; }

        /// <summary>
        /// Indica si al la guía fué entregada
        /// </summary>
        public bool Entregada { get; set; }

        /// <summary>
        /// Fecha en la que se hace la entrega
        /// </summary>
        public DateTime FechaEntrega { get; set; }

        /// <summary>
        /// Recibido de la notificación
        /// </summary>
        //public LIRecibidoGuia Recibidoguia { get; set; }

        public string DireccionAgenciaCiudadOrigen { get; set; }

        public string DireccionAgenciaCiudadDestino { get; set; }

        #region Mensajero

        private long idMensajero = 0;

        public long IdMensajero
        {
            get
            {
                return idMensajero;
            }
            set
            {
                idMensajero = value;
            }
        }

        public string NombreMensajero { get; set; }

        #endregion Mensajero

        #region Info Cliente Credito

        public int IdCliente { get; set; }

        public string NombreCliente { get; set; }

        public string NitCliente { get; set; }

        public int IdContrato { get; set; }

        public string NumeroContrato { get; set; }

        public int IdSucursal { get; set; }

        public string NombreSucursal { get; set; }

        public int? IdListaPrecios { get; set; }

        #endregion Info Cliente Credito

        public ADTrazaGuia TrazaGuiaEstado { get; set; }

        public bool EstaImpresa { get; set; }

        public int CantidadIntentosEntrega { get; set; }

        public int IdCaja { get; set; }

        public string IdentificacionRepLegal { get; set; }

        public string NombreRepLegal { get; set; }

        public Guid TokenClienteCredito { get; set; }

        public int TotalPaginas { get; set; }

        public string DescripcionEstado { get; set; }
        public string PuertaCasillero { get; set; }

        public int PesoKilos { get; set; }

        public ADFacturaDianDC FacturaDian { get; set; }

    }
}