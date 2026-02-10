using Framework.Servidor.Servicios.ContratoDatos;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    /// <summary>
    /// Suministros de un centros de servicio
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class SUSuministroCentroServicioDC : DataContractBase
    {

        /// <summary>
        /// Id del centro de servicio
        /// </summary>
        [DataMember]
        public long IdCentroServicio { get; set; }

        /// <summary>
        /// Informacion del suministro
        /// </summary>
        [DataMember]
        public IList<SUSuministro> Suministro { get; set; }
    }
}
