using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos
{
    /// <summary>
    /// Clase que contiene la Informacion de pago de un giro en caso de la devolucion del mismo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PGPagoPorDevolucionDC : DataContractBase
    {
        /// <summary>
        /// Es el valor del giro a pagar por una Devolucion
        /// </summary>
        [DataMember]
        public decimal ValorGiroAPagarPorDevolucion { get; set; }

        /// <summary>
        /// En Caso de Existir una devolucion se
        /// consulta el motivo de la devolucion
        /// </summary>
        [DataMember]
        public GIMotivoSolicitudDC MotivoSolicitud { get; set; }
    }
}