using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Contiene información de remitente y destinatario de acuerdo al tipo de cliente
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADMensajeriaTipoCliente : DataContractBase
    {
        /// <summary>
        /// Aplica solo para convenio - convenio
        /// </summary>
        [DataMember]
        public bool FacturaRemitente { get; set; }

        /// <summary>
        /// Aplica solo para convenio - convenio y convenio - peatón
        /// </summary>
        [DataMember]
        public ADConvenio ConvenioRemitente { get; set; }

        /// <summary>
        /// Aplica solo para Convenio - peatón y convenio - convenio
        /// </summary>
        [DataMember]
        public int? IdContratoConvenioRemitente { get; set; }

        /// <summary>
        /// Aplica solo para Convenio - Convenio y Peatón - Convenio
        /// </summary>
        [DataMember]
        public ADConvenio ConvenioDestinatario { get; set; }

        /// <summary>
        /// Aplica solo para Peatón - Peatón y Peatón - Convenio
        /// </summary>
        [DataMember]
        public ADPeaton PeatonRemitente { get; set; }

        /// <summary>
        /// Aplica solo para Peatón - Peatón
        /// </summary>
        [DataMember]
        public ADPeaton PeatonDestinatario { get; set; }
    }
}