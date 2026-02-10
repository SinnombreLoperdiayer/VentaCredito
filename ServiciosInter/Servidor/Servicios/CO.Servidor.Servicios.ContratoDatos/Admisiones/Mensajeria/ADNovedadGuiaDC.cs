using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADNovedadGuiaDC : DataContractBase
    {
        #region Datos Guia

        [DataMember]
        public ADGuia Guia { get; set; }

        #endregion Datos Guia

        #region Novedad

        [DataMember]
        public CCResponsableCambioDC ResponsableNovedad { get; set; }

        [DataMember]
        public string IdModulo { get; set; }

        [DataMember]
        public CCEnumTipoNovedadGuia TipoNovedad { get; set; }

        [DataMember]
        public long IdCodigoUsuario { get; set; }

        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "QuienSolicita", Description = "ToolTipQuienSolicita")]
        [DataMember]
        public string QuienSolicita { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
        public string Observaciones { get; set; }

        [DataMember]
        public bool ErrorDocFisico { get; set; }

        #endregion Novedad

        #region Novedad Cambio forma de pago

        /// <summary>
        /// Forma de pago a cambiar
        /// </summary>
        [DataMember]
        public ADGuiaFormaPago FormaPagoAnterior { get; set; }

        /// <summary>
        /// Forma de pago a cambiar
        /// </summary>
        [DataMember]
        public TAFormaPago FormaPagoNueva { get; set; }

        /// <summary>
        /// Cliente Credito cuando la nueva forma de pago es credito
        /// </summary>
        [DataMember]
        public CLClienteCreditoSucursalContrato ClienteCredito { get; set; }

        #endregion Novedad Cambio forma de pago

        /// <summary>
        /// Novedad a almacenar en el centro de servicios
        /// </summary>
        [DataMember]
        public PANovedadCentroServicioDCDeprecated NovedadCentroServicios { get; set; }
    }
}