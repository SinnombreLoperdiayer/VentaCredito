using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SUDatosResponsableSuministroDC
    {
        [DataMember]
        public long IdResponsableGuia { get; set; }

        [DataMember]
        public string NombreResponsable { get; set; }

        [DataMember]
        public string PrimerApellido { get; set; }

        [DataMember]
        public string SegundoApellido { get; set; }

        [DataMember]
        public string Identificacion { get; set; }

        [DataMember]
        public string Telefono { get; set; }

        [DataMember]
        public long IdCentroServicios { get; set; }

        [DataMember]
        public string NombreCentroServicio { get; set; }
        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public string IdLocalidad { get; set; }

        [DataMember]
        public string NombreLocalidad { get; set; }

        [DataMember]
        public string CodigoPostal { get; set; }

        [DataMember]
        public string TipoCentroServicios { get; set; }
    }
}
