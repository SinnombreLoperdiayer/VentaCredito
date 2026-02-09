using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGTerritorialDC
    {

        [DataMember]
        public long IdCentroLogistico { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string NombreTerritorial { get; set; }

        [DataMember]
        public decimal latitud { get; set; }

        [DataMember]
        public decimal Longitud { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string IdRegionalAdm { get; set; }

        [DataMember]
        public string IdMunicipio { get; set; }
    }
}
