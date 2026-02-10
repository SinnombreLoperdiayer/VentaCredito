using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class RURutaCWebDetalleCentrosServicios : DataContractBase
    {
        [DataMember]
        public string NombreLocalidadEstacion { get; set; }
        [DataMember]
        public string NombreCentroServicios { get; set; }
        [DataMember]
        public string Latitud { get; set; }
        [DataMember]
        public string Longitud { get; set; }
        [DataMember]
        public short Posicion { get; set; }
        [DataMember]
        public int IdLocalidadCoordenada { get; set; }
    }
}
