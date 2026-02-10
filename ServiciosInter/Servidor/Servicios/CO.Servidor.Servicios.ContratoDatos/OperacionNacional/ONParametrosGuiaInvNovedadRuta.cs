using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONParametrosGuiaInvNovedadRuta : DataContractBase
    {
        [DataMember]
        public int IdEstacionRuta { get; set; }
        [DataMember]
        public string NombreLocalidadEstacion { get; set; }
        [DataMember]
        public string IdCiudadDestino { get; set; }
        [DataMember]
        public string IdCiudadOrigen { get; set; }
        [DataMember]
        public long IdCentroServicioDestino { get; set; }
        [DataMember]
        public long IdCentroServicioOrigen { get; set; }
        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string NombreMensajero { get; set; }
        [DataMember]
        public string MOG_IdCiudadDestino { get; set; }
        [DataMember]
        public string NitConvenioRemitente { get; set; }
        [DataMember]
        public string RazonSocialConvenioRemitente { get; set; }
    }
}
