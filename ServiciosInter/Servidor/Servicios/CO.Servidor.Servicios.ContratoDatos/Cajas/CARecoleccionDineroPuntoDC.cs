using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene la informacion de
    /// la recoleccion de dinero de puntos
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CARecoleccionDineroPuntoDC : DataContractBase
    {
        /// <summary>
        /// Es el id recoleccion.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
        public int IdRecoleccion { get; set; }

        /// <summary>
        /// Es el id punto servicio.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdPuntoCentroServicio")]
        public long IdPuntoServicio { get; set; }

        /// <summary>
        /// El nombre punto.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
        public string NombrePunto { get; set; }

        /// <summary>
        /// Clase del punto Centro de Servicio del formulario manual
        /// </summary>
        [DataMember]

        // [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
        public PUCentroServicioReporte PuntoCentroServicio { get; set; }

        private string bolsaSeguridad;

        /// <summary>
        /// Es el Id de la bolsa seguridad.
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "TooltipBolsaSeguridad")]
        public string BolsaSeguridad
        {
            get { return bolsaSeguridad; }
            set
            {
                bolsaSeguridad = value;
            }
        }

        /// <summary>
        /// Valor total enviado por el Punto
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalReportado", Description = "ToolTipTotalAEmpresa")]
        public decimal ValorTotalEnviado { get; set; }

        /// <summary>
        /// Usuario que realiza el cierre
        /// </summary>
        [DataMember]
        public string UsuarioCierre { get; set; }

        /// <summary>
        /// es le mensajero que recoge el dinero
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreMensajero")]
        public OUNombresMensajeroDC MensajeroPunto { get; set; }

        /// <summary>
        /// Observacion del dinero recaudado
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
        public string Observacion { get; set; }

        private decimal valorReal;

        /// <summary>
        /// Valor real recibido e ingresado por el usuario
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorReal")]
        public decimal ValorReal
        {
            get { return valorReal; }
            set
            {
                valorReal = value;
                if (ValorTotalEnviado != 0)
                {
                    ValorDiferencia = ValorTotalEnviado - valorReal;
                }
            }
        }

        private decimal valorDiferencia;

        /// <summary>
        /// Es el Valor de la diferencia entre el
        /// Valor TotalAReportar y TotalReportado
        /// </summary>
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorDeDiferencia")]
        public decimal ValorDiferencia
        {
            get { return valorDiferencia; }
            set
            {
                valorDiferencia = value;
                if (value != 0 && OnDiferenciaEncontradaEvent != null)
                {
                    OnDiferenciaEncontradaEvent();
                }
            }
        }

        /// <summary>
        /// Es el tipo de Observacion
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoObsevPunto", Description = "ToolTipTipoObsevPunto")]
        public CATipoObsPuntoAgenciaDC TipoObservacionPunto { get; set; }

        /// <summary>
        /// Fecha en la que se recolecto el dinero en el punto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaRecoleccion", Description = "TootipFechaRecoleccion")]
        public DateTime FechaRecoleccion { get; set; }

        /// <summary>
        /// Es el registro si es manual o enviado
        /// </summary>
        [DataMember]
        public bool RegistroManual { get; set; }

        /// <summary>
        /// Es el registro en caja del punto que Reporta
        /// </summary>
        [DataMember]
        public CARegistroTransacCajaDC RegistroCajaPuntoReporta { get; set; }

        /// <summary>
        /// Es el registro en caja de la Agencia
        /// </summary>
        [DataMember]
        public CARegistroTransacCajaDC RegistroCajaAgencia { get; set; }

        /// <summary>
        /// registra el Movimiento en la tbl MovCentroSvcCentroSvc
        /// </summary>
        [DataMember]
        public CAMovCentroSvcCentroSvcDC RegistroMovEntreCentroSvc { get; set; }

        /// <summary>
        /// Es la Transaccion asociada al mensajero
        /// </summary>
        [DataMember]
        public CAReporteMensajeroCajaDC RegistroMovimentoAReporteMensajero { get; set; }

        /// <summary>
        /// Es el id del ultimo cierre
        /// </summary>
        [DataMember]
        public long IdCierreCentroServicios { get; set; }

        /// <summary>
        /// Es el suministro a descargar por novasoft
        /// no es obligatorio el suministro se consume al
        /// momento de guardar la transacción.
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SuministroPlanillaDespachoManual",
          Description = "ToolTipSuministroPlanillaDespachoManual")]
        public long PlanillaDespachoManual { get; set; }

        #region Eventos

        /// <summary>
        /// Evento cuando se genera una direncia entre el vr enviado y el real registrado
        /// </summary>
        public delegate void OnDiferenciaEncontrada();

        public event OnDiferenciaEncontrada OnDiferenciaEncontradaEvent;

        #endregion Eventos
    }
}