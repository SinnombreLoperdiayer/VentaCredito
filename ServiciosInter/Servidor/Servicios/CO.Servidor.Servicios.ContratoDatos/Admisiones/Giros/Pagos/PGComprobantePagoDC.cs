using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos
{
    /// <summary>
    /// Clase con informacion de retorno para pagar un giro
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PGComprobantePagoDC : DataContractBase
    {
        [DataMember]
        public long IdComprobantePago { get; set; }

        /// <summary>
        /// Fecha de grabacion
        /// </summary>
        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Indica si el usuario debe diligenciar el formato de declaracion voluntaria de fondos
        /// </summary>
        [DataMember]
        public bool ObligaDeclaracionVoluntariaFondos { get; set; }

        [DataMember]
        public PGPagoPorDevolucionDC PagoPorDevolucion { get; set; }
    }
}