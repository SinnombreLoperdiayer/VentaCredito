using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Recogidas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public  class RGMensajeroLocalidadDC
    {
        [DataMember]
        public long IdMensajero { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string PrimerApellido { get; set; }
        [DataMember]
        public string SegundoApellido { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public int IdTipoMensajero { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public long? IdVehiculo { get; set; }
        [DataMember]
        public string placa { get; set; }
        [DataMember]        
        public int? IdTipoContrato { get; set; }
        [DataMember]
        public string NombreTipoContrato{ get; set; }
        [DataMember]
        public string Propiedad { get; set; }        

        [DataMember]
        public string NombreCompleto { get; set; }
        [DataMember]
        public string Identificacion { get; set; }

    }
}
