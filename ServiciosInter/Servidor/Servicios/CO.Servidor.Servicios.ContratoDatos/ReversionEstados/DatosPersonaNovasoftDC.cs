using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ReversionEstados
{
    [DataContract]
    public class DatosPersonaNovasoftDC
    {
        [DataMember]
        public string IdCargo { get; set; }
        [DataMember]
        public string IdCargoNovasoft { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public bool Estado { get; set; }
        [DataMember]
        public int IdProcedimiento { get; set; }
        [DataMember]
        public string NombreCompleto { get; set; }
        [DataMember]
        public long NumeroDocumento { get; set; }
        [DataMember]
        public string TipoIdentificacion { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Proceso { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public string DescripcionTarea { get; set; }
        [DataMember]
        public DateTime FechaEjecucionTarea { get; set; }
    }
}
