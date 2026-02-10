using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADOrdenServicioMasivoDC : DataContractBase
    {
        [DataMember]
        public long IdOrdenServicioMasivo { get; set; }

        /// <summary>
        /// Es la fecha en la que se crea
        /// la orden de servicio
        /// </summary>
        [DataMember]
        public DateTime FechaOrdenServicio { get; set; }

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public string NombreCentroServicios { get; set; }

        [DataMember]
        public int IdSucursalCliente { get; set; }

        [DataMember]
        public string NombreSucursalCliente { get; set; }
    }
}