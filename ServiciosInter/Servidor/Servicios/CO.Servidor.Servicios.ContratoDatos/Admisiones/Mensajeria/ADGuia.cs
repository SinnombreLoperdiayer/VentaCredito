using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using System.Linq;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuia : DataContractBase
    {
        [DataMember]
        public long IdTrazaGuia { get; set; }

        [DataMember]
        public CLClienteContadoDC Remitente { get; set; }

        [DataMember]
        public CLClienteContadoDC Destinatario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
        public long IdAdmision { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long NumeroGuiaDHL { get; set; }

        [DataMember]
        public ADGuiaInternacionalDC DatosGuiaInternacional { get; set; }


        [DataMember]
        public string PrefijoNumeroGuia { get; set; }

        [DataMember]
        public string DigitoVerificacion { get; set; }

        [DataMember]
        public string GuidDeChequeo { get; set; }

        [DataMember]
        public bool EsAutomatico { get; set; }

        public string TipoTarifa { get { return EsAutomatico == true ? "Automático" : "Manual"; } }

        [DataMember]
        public string IdUnidadNegocio { get; set; }

        [DataMember]
        public int IdServicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "LineaServicio", Description = "TooltipLineaServicio")]
        public string NombreServicio { get; set; }

        [DataMember]
        public string IdTipoEntrega { get; set; }

        [DataMember]
        public string DescripcionTipoEntrega { get; set; }

        [DataMember]
        public long IdCentroServicioOrigen { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCentroServicioOrigen")]
        public string NombreCentroServicioOrigen { get; set; }

        [DataMember]
        public long IdCentroServicioDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCentroServiciosDestino")]
        public string NombreCentroServicioDestino { get; set; }

        [DataMember]
        public string IdPaisOrigen { get; set; }

        [DataMember]
        public string NombrePaisOrigen { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCiudadOrigen", Description = "TooltipIdCiudadOrigen")]
        public string IdCiudadOrigen { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadOrigen", Description = "TooltipCiudadOrigen")]
        public string NombreCiudadOrigen { get; set; }

        [DataMember]
        public string CodigoPostalOrigen { get; set; }

        [DataMember]
        public string IdPaisDestino { get; set; }

        [DataMember]
        public string NombrePaisDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCiudadDestino", Description = "ToolTipIdLocalidadDestino")]
        public string IdCiudadDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadDestino", Description = "ToolTipLocalidadDestino")]
        public string NombreCiudadDestino { get; set; }

        [DataMember]
        public string CodigoPostalDestino { get; set; }

        [DataMember]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        public ADEnumTipoCliente TipoCliente { get; set; }

        [DataMember]
        public string TipoClienteStr { get; set; }

        [DataMember]
        public short DiasDeEntrega { get; set; }

        [DataMember]
        public DateTime FechaEstimadaEntrega { get; set; }

        [DataMember]
        public DateTime FechaEstimadaEntregaNew { get; set; }

        [DataMember]
        public DateTime FechaEstimadaDigitalizacion { get; set; }

        [DataMember]
        public DateTime FechaEstimadaArchivo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTransporte", Description = "TooltipValorTransporte")]
        public decimal ValorAdmision { get; set; }

        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "Valor")]
        [DataMember]
        public decimal ValorTotal { get; set; }

        [DataMember]
        public decimal ValorTotalImpuestos { get; set; }

        [DataMember]
        public decimal ValorTotalRetenciones { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimaSeguro", Description = "TooltipPrimaSeguro")]
        public decimal ValorPrimaSeguro { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorEmpaque")]
        public decimal ValorEmpaque { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAdicionales", Description = "TooltipValorAdicionales")]
        public decimal ValorAdicionales { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAsegurado", Description = "TooltipValorComercial")]
        public decimal ValorDeclarado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DiceContener", Description = "TooltipDiceContener")]
        public string DiceContener { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public short NumeroPieza { get; set; }

        [DataMember]
        public short TotalPiezas { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaAdmision", Description = "TooltipFechaAdmision")]
        public DateTime FechaAdmision { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "TooltipPeso")]
        public decimal Peso { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoVolumetricoCorto", Description = "TooltipPesoVolumetrico")]
        public decimal PesoLiqVolumetrico { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "TooltipPeso")]
        public decimal PesoLiqMasa { get; set; }

        [DataMember]
        public bool EsPesoVolumetrico { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "TooltipBolsaSeguridad")]
        public string NumeroBolsaSeguridad { get; set; }

        [DataMember]
        public short? IdMotivoNoUsoBolsaSegurida { get; set; }

        [DataMember]
        public string MotivoNoUsoBolsaSeguriDesc { get; set; }

        [DataMember]
        public string NoUsoaBolsaSeguridadObserv { get; set; }

        [DataMember]
        public string IdUnidadMedida { get; set; }

        [DataMember]
        public decimal Largo { get; set; }

        [DataMember]
        public decimal Ancho { get; set; }

        [DataMember]
        public decimal Alto { get; set; }

        [DataMember]
        public string NoPedido { get; set; }

        [DataMember]
        public bool EsRecomendado { get; set; }

        [DataMember]
        public short IdTipoEnvio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoEnvio", Description = "TooltipTipoEnvio")]
        public string NombreTipoEnvio { get; set; }

        [DataMember]
        public bool AdmisionSistemaMensajero { get; set; }

        /// <summary>
        /// Con base en las formas de pago seleccionadas se carga esta bandera en el servidor
        /// </summary>
        [DataMember]
        public bool EsAlCobro { get; set; }

        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha")]
        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string CreadoPor { get; set; }

        [DataMember]
        public string ObservacionEstadoGuia { get; set; }

        [DataMember]
        public ADEnumEstadoGuia EstadoGuia { get; set; }

        [DataMember]
        public List<TAValorAdicional> ValoresAdicionales { get; set; }

        [DataMember]
        public List<ADGuiaFormaPago> FormasPago { get; set; }


        public string NombreFormaPago
        {
            get
            {
                if (FormasPago.FirstOrDefault() != null)
                    return FormasPago.FirstOrDefault().Descripcion;
                return "";
            }
        }

        /// <summary>
        /// Contiene el id del concepto de caja asociado al servicio
        /// </summary>
        [DataMember]
        public int IdConceptoCaja { get; set; }

        /// <summary>
        /// Valor del servicio o valor de la tarifa
        /// </summary>
        [DataMember]
        public decimal ValorServicio { get; set; }

        /// <summary>
        /// Indica si al momento de grabar la transacción la guía fué pagada
        /// </summary>
        [DataMember]
        public bool EstaPagada { get; set; }

        /// <summary>
        /// Fecha en la que se hace el pago de la guía
        /// </summary>
        [DataMember]
        public DateTime? FechaPago { get; set; }

        /// <summary>
        /// Caja de carton en la que se almacena la guia archivada. OJO:Esta no es la caja del punto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Caja")]
        public long Caja { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Lote")]
        public int Lote { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Posicion")]
        public int Posicion { get; set; }

        [DataMember]
        public bool Aprobada { get; set; }

        [DataMember]
        public bool NoAprobada { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoEntrega", Description = "TooltipMotivoEntrega")]
        public string MotivoEntrega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago")]
        public string FormasPagoDescripcion { get; set; }

        [DataMember]
        public string FormasPagoIds { get; set; }

        /// <summary>
        /// Es el id del usuario. Rafram 14-09-2012
        /// </summary>
        [DataMember]
        public long IdCodigoUsuario { get; set; }

        [DataMember]
        public bool Supervision { get; set; }

        [DataMember]
        public DateTime FechaSupervision { get; set; }

        [DataMember]
        public bool NotificarEntregaPorEmail { get; set; }

        /// <summary>
        /// Indica si al la guía fué entregada
        /// </summary>
        [DataMember]
        public bool Entregada { get; set; }

        /// <summary>
        /// Fecha en la que se hace la entrega
        /// </summary>
        [DataMember]
        public DateTime FechaEntrega { get; set; }


        /// <summary>
        /// Recibido de la notificación
        /// </summary>
        [DataMember]
        public LIRecibidoGuia Recibidoguia { get; set; }

        [DataMember]
        public string DireccionAgenciaCiudadOrigen { get; set; }

        [DataMember]
        public string DireccionAgenciaCiudadDestino { get; set; }
        #region Mensajero

        private long idMensajero = 0;

        [DataMember]
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

        [DataMember]
        public string NombreMensajero { get; set; }

        #endregion Mensajero

        #region Info Cliente Credito

        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public string NombreCliente { get; set; }

        [DataMember]
        public string NitCliente { get; set; }

        [DataMember]
        public int IdContrato { get; set; }

        [DataMember]
        public string NumeroContrato { get; set; }

        [DataMember]
        public int IdSucursal { get; set; }

        [DataMember]
        public string NombreSucursal { get; set; }

        [DataMember]
        public int? IdListaPrecios { get; set; }

        #endregion Info Cliente Credito

        [DataMember]
        public ADTrazaGuia TrazaGuiaEstado { get; set; }

        [DataMember]
        public bool EstaImpresa { get; set; }

        [DataMember]
        public int CantidadIntentosEntrega { get; set; }


        [DataMember]
        public int IdCaja { get; set; }

        [DataMember]
        public string IdentificacionRepLegal { get; set; }

        [DataMember]
        public string NombreRepLegal { get; set; }

        [DataMember]
        public Guid TokenClienteCredito { get; set; }

        [DataMember]
        public int TotalPaginas { get; set; }

        [DataMember]
        public string DescripcionEstado { get; set; }

        [DataMember]
        public long IdCentroServicioEstado { get; set; }

        [DataMember]
        public string NombreCentroServicioEstado { get; set; }
    }

}