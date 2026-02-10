using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAAccionDC
    {
        [DataMember]
        public Byte IdAccion { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
