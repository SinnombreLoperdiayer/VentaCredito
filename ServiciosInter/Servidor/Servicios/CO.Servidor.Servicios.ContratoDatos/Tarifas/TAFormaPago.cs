using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    /// <summary>
    /// Clase que contiene la información de la forma de pago de los servicios
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TAFormaPago : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id", Description = "TooltipId")]
        public short IdFormaPago { get; set; }

        /// <summary>
        /// Propiedad establecida para binding en Parametrizacion de Servicios Novasoft
        /// </summary>
        [DataMember]
        public int IdFormaPagoInt { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago", Description = "TooltipFormaPagoFact")]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Asignada { get; set; }

        [DataMember]
        public bool Actual { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Indica si la forma de pago acepta mixto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "AceptaPagoMixto", Description = "ToolTipAceptaPagoMixto")]
        public bool AceptaMixto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor", Description = "ToolTipValor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Indica si la forma aplica para cliente
        /// </summary>
        [DataMember]
        public bool AplicaFactura { get; set; }

        /// <summary>
        /// Es el numero asociado a la forma de pago.
        /// Pricipalmente para el PinPrepago y
        /// Cheque.
        /// </summary>
        /// <value>
        /// Es el numero asociado a la forma de pago.
        /// Pricipalmente para el PinPrepago y
        /// Cheque.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroAsociadoFormaPago", Description = "ToolTipNumeroAsociado")]
        public string NumeroAsociadoFormaPago { get; set; }

        [DataMember]
        public System.Collections.Generic.List<TAServicioDC> ServiciosAsociados { get; set; }
    }
}