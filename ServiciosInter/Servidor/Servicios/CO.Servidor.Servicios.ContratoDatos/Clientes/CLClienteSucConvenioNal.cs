using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CLClienteSucConvenioNal : DataContractBase
    {
        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public string Nit { get; set; }

        [DataMember]
        public string DigitoVerificacion { get; set; }

        [DataMember]
        public string RazonSocial { get; set; }

        [DataMember]
        public int IdSucursal { get; set; }

        [DataMember]
        public string NombreSucursal { get; set; }

        [DataMember]
        public string DireccionSucursal { get; set; }

        [DataMember]
        public string TelefonoSucursal { get; set; }

        [DataMember]
        public string NumeroContrato { get; set; }

        [DataMember]
        public string NombreContrato { get; set; }

        [DataMember]
        public int IdListaPrecios { get; set; }

        [DataMember]
        public int IdContrato { get; set; }


        public string Nombre
        {
            get
            {
                return string.Join("-", this.RazonSocial, this.NombreContrato, this.DireccionSucursal);
            }
        }
    }
}
