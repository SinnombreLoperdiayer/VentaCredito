using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos
{
    /// <summary>
    /// Clase que contiene la información de la admisión y el pago de un giro
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PGPagoAdmisionGiroDC : DataContractBase
    {
        [DataMember]
        public GIAdmisionGirosDC Admision { get; set; }

        [DataMember]
        public PGPagosGirosDC Pago { get; set; }

        [DataMember]
        public bool Aprobada { get; set; }

        [DataMember]
        public bool NoAprobada { get; set; }
    }
}