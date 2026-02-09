using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Integraciones.Satrack
{
    [DataContract(Namespace = "")]
    public class INRegionescargueDC
    {
        [DataMember]
        public List<string> CodigoRegion { get; set; }
    }
}