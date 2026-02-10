using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;



namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  /// <summary>
  /// Clase con los parametros de
  /// la vista de EnviosPendientesMensajeros_VOPU
  /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUEnviosPendMensajerosDC : DataContractBase
    {
        /// <summary>
        /// Bandera de Id si es al cobro o no
        /// </summary>
        /// <value>
        ///   <c>true</c> if [al cobro]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool AlCobro { get; set; }

        /// <summary>
        /// Es el estado de la Entrega.
        /// </summary>
        /// <value>
        /// The estado entregada.
        /// </value>
        [DataMember]
        public string EstadoEntregada { get; set; }

        /// <summary>
        /// Es el Numero de la Guia
        /// </summary>
        /// <value>
        /// The numero guia.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia")]
        public long NumeroGuia { get; set; }

        /// <summary>
        /// Es le Valor de la Guia
        /// </summary>
        /// <value>
        /// The valor total guia.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorAlCobro")]
        public decimal ValorTotalGuia { get; set; }

        /// <summary>
        /// Es el numero de la Planilla
        /// </summary>
        /// <value>
        /// The numero planilla.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla")]
        public long NumeroPlanilla { get; set; }

        /// <summary>
        /// Informacion del Mensajero
        /// </summary>
        /// <value>
        /// The mensajero.
        /// </value>
        [DataMember]
        public OUNombresMensajeroDC Mensajero { get; set; }


        /// <summary>
        /// Indica si el al cobro está o no descargado
        /// </summary>
        [DataMember]
        public bool Descargado { get; set; }

        /// <summary>
        /// Fecha en la cual fué planillado el envío al mensajero
        /// </summary>
        [DataMember]
        public DateTime FechaPlanilla { get; set; }

        [DataMember]
        public bool AfectadoEnCaja { get; set; }

        [DataMember]
        public string NoComprobantePago { get; set; }
    }
}