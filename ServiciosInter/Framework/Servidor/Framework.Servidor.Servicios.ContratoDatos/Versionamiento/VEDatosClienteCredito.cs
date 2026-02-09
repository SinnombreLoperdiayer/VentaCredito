using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class VEDatosClienteCredito : DataContractBase
    {
        [DataMember]
        public string SucursalNombre { get; set; }

        [DataMember]
        public int SucursalId { get; set; }

        [DataMember]
        public int ClienteId { get; set; }

        [DataMember]
        public string ClienteNombre { get; set; }

        [DataMember]
        public string NitCliente { get; set; }

        [DataMember]
        public string DigitoVerificacionCliente { get; set; }

        [DataMember]
        public string TelefonoCliente { get; set; }

        [DataMember]
        public string DireccionCliente { get; set; }

        [DataMember]
        public string IdPais { get; set; }

        [DataMember]
        public string DescripcionPais { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public string DescripcionCiudad { get; set; }

        [DataMember]
        public string CodigoPostal { get; set; }
    }
}