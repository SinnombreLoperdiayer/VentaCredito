using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAListaParametrosAcumulativasDC
    {
        [DataMember]
        public List<RADetalleParametrosAcumulativasDC> ListaParametros { get; set; }
    }
}
