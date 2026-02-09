using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CARegistrarEnvioAgenciaDC : DataContractBase
    {
        /// <summary>
        /// es el valor a enviar por parte del punto a la Agencia
        /// </summary>
        [DataMember]
        public decimal ValorAEnviarPorPunto { get; set; }

        /// <summary>
        /// es el ultimo cierre generado por el punto
        /// </summary>
        [DataMember]
        public CACierreCentroServicioDC InfoUltimoCierrePunto { get; set; }

        /// <summary>
        /// Lista de mensajeros de la agencia a la que pertenece el punto
        /// </summary>
        [DataMember]
        public IEnumerable<OUNombresMensajeroDC> ListMensajerosAgencia { get; set; }
    }
}