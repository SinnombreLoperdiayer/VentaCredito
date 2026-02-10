using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SECentroServicio : DataContractBase
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public int Caja { get; set; }

        /// <summary>
        /// Descripcion del Nombre completo
        /// </summary>        
        public string FullDescripcion
        {
            get
            {
                return string.Format("{0} | {1}", Id, Descripcion);
            }
        }

        [DataMember]
        public string IdCentroCostos { get; set; }

        [DataMember]
        public bool ImpresionPos { get; set; }
    }
}