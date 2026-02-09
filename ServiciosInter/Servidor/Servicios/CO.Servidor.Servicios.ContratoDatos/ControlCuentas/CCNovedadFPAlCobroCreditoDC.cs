using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCNovedadFPAlCobroCreditoDC : ADNovedadGuiaDC
    {
        /// <summary>
        /// Id del cliente convenio al cual se le va a asignar la guía
        /// </summary>
        [DataMember]
        public long IdClienteConvenio { get; set; }

        /// <summary>
        /// Id del contrato del cliente al cual se le va a asignar la guía
        /// </summary>
        [DataMember]
        public int IdContrato { get; set; }

        /// <summary>
        /// Id de la sucursal del cliente a la cual se le va a asignar la guía
        /// </summary>
        [DataMember]
        public int IdSucursal { get; set; }
    }
}
