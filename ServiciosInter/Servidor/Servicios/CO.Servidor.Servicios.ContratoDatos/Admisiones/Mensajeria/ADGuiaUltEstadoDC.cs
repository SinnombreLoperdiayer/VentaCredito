using System.Collections.Generic;
using System.Runtime.Serialization;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADGuiaUltEstadoDC
    {
        [DataMember]
        public long IdRacol { get; set; }

        [DataMember]
        public ADGuia Guia { get; set; }

        [DataMember]
        public ADTrazaGuia TrazaGuia { get; set; }

        [DataMember]
        public List<ADGuiaFormaPago> FormasPago { get; set; }
    }
}