using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RACargoEscalarDC
    {
        [DataMember]        
        public string DocumentoEmpleado { get; set; }

        [DataMember]
        public string Sucursal { get; set; }

        [DataMember]
        public string IdCargoNovasoft { get; set; }

        [DataMember]
        public string IdCargoController { get; set; }

        [DataMember]
        public string NombreEmpleado { get; set; }

        [DataMember]
        public string Correo { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public List<RAHorarioEmpleadoDC> HorarioEmpleado { get; set; }

        [DataMember]
        public string CodPlantaNovasoft { get; set; }

    }
}
