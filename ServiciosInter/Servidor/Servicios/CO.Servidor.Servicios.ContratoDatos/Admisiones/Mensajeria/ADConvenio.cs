using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADConvenio : DataContractBase
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nit { get; set; }

        [DataMember]
        public string RazonSocial { get; set; }

        [DataMember]
        public string Telefono { get; set; }

        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string EMail { get; set; }

        [DataMember]
        public int? IdSucursalRecogida { get; set; }

        [DataMember]
        public int Contrato { get; set; }

        [DataMember]
        public int? IdListaPrecios { get; set; }
    }
}