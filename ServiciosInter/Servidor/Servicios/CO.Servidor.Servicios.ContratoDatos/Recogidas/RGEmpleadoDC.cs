using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RGEmpleadoDC
    {
        [DataMember]
        public string nombreEmpleado { get; set; }

        [DataMember]
        public string idTipoIdentificacion { get; set; }

        [DataMember]
        public string idEmpleado { get; set; }

        [DataMember]
        public string email { get; set; }

        [DataMember]
        public string idCargo { get; set; }

        [DataMember]
        public string descripcionCargo { get; set; }

        [DataMember]
        public string direccion { get; set; }

        [DataMember]
        public string telefonoFijo { get; set; }

        [DataMember]
        public string telefonoCelular { get; set; }

        [DataMember]
        public string ciudadEmpleado { get; set; }

        [DataMember]
        public string racolEmpleado { get; set; }

        [DataMember]
        public bool estadoEmpleado { get; set; }

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public PUEnumTipoCentroServicioDC TipoCentroServicio { get; set; }        

    }
}
