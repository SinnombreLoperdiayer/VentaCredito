using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAListarCargoEscalonamientoParametrizacionRapsDC
    {
        //[DataMember]
        //public int NumeroEscalonamiento { get; set; }

        [DataMember]
        public int IdParametrizacionRap { get; set; }

        [DataMember]
        public int idCargo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public int orden { get; set; }

        [DataMember]
        public int IdTipoHora { get; set; }

        [DataMember]
        public int HorasEscalar { get; set; }
    }
}
