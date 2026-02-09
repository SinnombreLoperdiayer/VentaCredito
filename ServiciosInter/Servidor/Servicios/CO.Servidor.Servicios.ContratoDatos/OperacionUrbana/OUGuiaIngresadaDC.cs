using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUGuiaIngresadaDC : DataContractBase
    {
        /// <summary>
        /// retorna o asigna el nombre completo del mensajero
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("NombreCompleto")]
        [Filtrable("NombreCompleto", "Nombre: ", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "Nombre")]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// retorna o asigna el numero de identificacion del mensajero
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("PEI_Identificacion")]
        [Filtrable("PEI_Identificacion", "Identificacion: ", COEnumTipoControlFiltro.TextBox)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "Identificacion")]
        public string Identificacion { get; set; }

        /// <summary>
        /// retorna o asigna el id del mensajero
        /// </summary>
        [DataMember]
        public long IdMensajero { get; set; }

        /// <summary>
        /// retorna o asigna la fecha del ingreso al centro logistico
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
        public DateTime FechaActual { get; set; }

        /// <summary>
        /// Retorna o asigna el número de guía
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Guia", Description = "TooltipGuia")]
        public long? NumeroGuia { get; set; }

        /// <summary>
        /// Retorna o asigna el número de guía
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "GuiaRotulo", Description = "ToolTipGuiaRotulo")]
        public string NumeroGuiaRotulo { get; set; }

        /// <summary>
        /// retorna o asigna los estados del empaque de mensajeria y carga mayor a 1 Kg
        /// </summary>
        [DataMember]
        public OUEstadosEmpaqueDC EstadoEmpaqueMayorUnKG { get; set; }

        /// <summary>
        /// retorna o asigna los estados del empaque de mensajeria y carga menor a 1 Kg
        /// </summary>
        [DataMember]
        public OUEstadosEmpaqueDC EstadoEmpaqueMenorUnKG { get; set; }

        /// <summary>
        /// retorna o asigna el estado del mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public OUEstadosMensajeroDC Estado { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad del centro logistico
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Ciudad { get; set; }

        /// <summary>
        /// retorna o asigna el peso capturado al ingreso al centro logistico
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "TooltipPeso")]
        public decimal? Peso { get; set; }

        /// <summary>
        /// retorna o asigna el numero de la pieza
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPiezas")]
        public short? NumeroPiezas { get; set; }

        /// <summary>
        /// retorna o asigna el total de las piezas
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalPiezas")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public short? TotalPiezas { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de cliente
        /// </summary>
        [DataMember]
        public string TipoCliente { get; set; }

        /// <summary>
        /// retorna o asigna el si la guia fue ingresada de manera automatica
        /// </summary>
        [DataMember]
        public bool GuiaAutomatica { get; set; }

        /// <summary>
        /// retorna o asigna el id de admision mensajeria
        /// </summary>
        [DataMember]
        public long IdAdmision { get; set; }

        /// <summary>
        /// retorna o asigna el id del Centro logistico
        /// </summary>
        [DataMember]
        public long IdCentroLogistico { get; set; }

        /// <summary>
        /// retorna o asigna el nombre del Centro logistico
        /// </summary>
        [DataMember]
        public string NombreCentroLogistico { get; set; }

        /// <summary>
        /// retorna o asigna si la admision es mayor
        /// </summary>
        [DataMember]
        public bool MayorUnKg { get; set; }

        /// <summary>
        /// retorna o asigna el detalle de la guia ingresada
        /// </summary>
        [DataMember]
        public OUDetalleGuiaDC DetalleGuia { get; set; }

        /// <summary>
        /// retorna o asigna peso capturado en el sistema
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Peso", Description = "TooltipPeso")]
        public decimal PesoSistema { get; set; }

        /// <summary>
        /// retorna o asigna la planilla de la guia ingresada
        /// </summary>
        [DataMember]
        public long Planilla { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de la planilla
        /// </summary>
        [DataMember]
        public DateTime FechaPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna si el envio fue ingresado por planilla
        /// </summary>
        [DataMember]
        public bool IngresoPorPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el id del contrato del remitente
        /// </summary>
        [DataMember]
        public int IdContratoRemitente { get; set; }

        /// <summary>
        /// Retorna o asigna la sucursal del remitente
        /// </summary>
        [DataMember]
        public int IdSucursal { get; set; }

        /// <summary>
        /// Retorna o asigna el valor total del envio
        /// </summary>
        [DataMember]
        public decimal ValorTotal { get; set; }

        /// <summary>
        /// Retorna o asigna el total de los envios descargados por mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EnviosDescargados", Description = "EnviosDescargados")]
        public int TotalEnvios { get; set; }

        /// <summary>
        /// Retorna o asigna si la guia esta registrada en el sistema
        /// </summary>
        [DataMember]
        public bool GuiaRegistrada { get; set; }

        /// <summary>
        /// Retorna o asigna el id de la forma de pago
        /// </summary>
        [DataMember]
        public short IdFormaPago { get; set; }

        /// <summary>
        /// Retorna o asigna la descripcion de la forma de pago
        /// </summary>
        [DataMember]
        public string FormaPagoDesc { get; set; }

        /// <summary>
        /// Retorna o asigna si la guia esta descargada
        /// </summary>
        [DataMember]
        public bool EstaDescargada { get; set; }

        /// <summary>
        /// Retorna o asigna el consecutivo de la guia
        /// </summary>
        [DataMember]
        public short Consecutivo { get; set; }

        /// <summary>
        /// retorna o asigna si el mensajero recoge dinero por el envio
        /// </summary>
        [DataMember]
        public bool RecogeDinero { get; set; }

        /// <summary>
        /// Retorna o asigna la cantidad de envios radicados en la planilla
        /// </summary>
        [DataMember]
        public short CantidadRadicados { get; set; }

        /// <summary>
        /// Retorna o asigna si el envio está verificado o no
        /// </summary>

        private bool estaVerificada;
        [DataMember]
        public bool EstaVerificada
        {
            get { return estaVerificada; }
            set { estaVerificada = value; OnPropertyChanged("EstaVerificada"); }
        }

        /// <summary>
        /// Retorno a asigna quien realizo la verificacion de envio
        /// </summary>
        [DataMember]
        public string VerificadaPor { get; set; }

        /// <summary>
        /// Retorno a asigna quien realizo la verificacion de envio
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        /// <summary>
        /// Retorna o asigna el teléfono del destinatario
        /// </summary>
        [DataMember]
        public string TelefonoDestinatario { get; set; }

        /// <summary>
        /// Retorna o asigna la dirección del destinatario
        /// </summary>
        [DataMember]
        public string DireccionDestinatario { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        [DataMember]
        public short IdEstadoGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia en la planilla
        /// </summary>
        [DataMember]
        public string EstadoGuiaPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna la descripcion del estado de la guia en la planilla
        /// </summary>
        [DataMember]
        public string EstadoDescripcionGuiaPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el id de la ciudad del centro logistico
        /// </summary>
        [DataMember]
        public string IdCiudad { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
        public string Observaciones { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo", Description = "TooltipMotivo")]
        public ADMotivoGuiaDC Motivo { get; set; }

        [DataMember]
        public string NuevoEstadoGuia { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public string DescripcionNuevoEstadoGuia { get; set; }

        [DataMember]
        public string EstadoGuia { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public string DescripcionEstadoGuia { get; set; }

        [DataMember]
        public bool EstaPlanillada { get; set; }

        [DataMember]
        public LIEvidenciaDevolucionDC EvidenciaDevolucion { get; set; }

        /// <summary>
        /// Retorna o asigna el id del centro de servicio origen
        /// </summary>
        [DataMember]
        public long IdCentroServicioOrigen { get; set; }

        /// <summary>
        /// retorna o asigna el nombre del centro de servicios origen
        /// </summary>
        [DataMember]
        public string NombreCentroServicioOrigen { get; set; }

        /// <summary>
        /// Retorna o asigna el id del centro de servicio destino
        /// </summary>
        [DataMember]
        public long IdCentroServicioDestino { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre del centro de servicio destino
        /// </summary>
        [DataMember]
        public string NombreCentroServicioDestino { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue")]
        public DateTime FechaDescarga { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string UsuarioDescarga { get; set; }

        [DataMember]
        public string FechaDescargaString { get; set; }

        [DataMember]
        public LIRecibidoGuia Notificacion { get; set; }

        [DataMember]
        public bool DescargueSupervisado { get; set; }

        [DataMember]
        public bool EsAlCobro { get; set; }

        [DataMember]
        public bool EstaPagada { get; set; }

        [DataMember]
        public bool RadicadoVerificado { get; set; }

        [DataMember]
        public ADEnumTipoImpreso TipoImpreso { get; set; }

        [DataMember]
        public string TipoDespacho { get; set; }

        [DataMember]
        public short CantidadReintentosEntrega { get; set; }

        [DataMember]
        public string NombreTipoEnvio { get; set; }

        [DataMember]
        public string DiceContener { get; set; }

        [DataMember]
        public long NumeroAuditoria { get; set; }

        [DataMember]
        public DateTime FechaAsignacion { get; set; }

        [DataMember]
        public DateTime FechaAuditoria { get; set; }

        [DataMember]
        public int EsSobrante { get; set; }

        [DataMember]
        public string UsuarioAuditor { get; set; }

        /// <summary>
        /// retorna o asigna la fecha del ingreso al centro logistico
        /// </summary>
        [DataMember]
        public DateTime FechaMotivoDevolucion { get; set; }


        /// <summary>
        /// retorna o asigna la novedad de la guia en la devolucion
        /// </summary>
        [DataMember]
        public COTipoNovedadGuiaDC Novedad { get; set; }

        /// <summary>
        /// Retorna si la guia es reclame en oficina
        /// </summary>
        [DataMember]
        public bool EsReclameEnOficina { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PALocalidadDC CiudadDestino { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TAServicioDC Servicio { get; set; }

        [DataMember]
        public string NumeroPedido { get; set; }

        [DataMember]
        public int IdCliente { get; set; }
    }
}