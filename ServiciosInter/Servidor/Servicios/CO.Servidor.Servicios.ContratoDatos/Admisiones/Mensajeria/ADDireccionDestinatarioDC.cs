using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract]
    public class ADDireccionDestinatarioDC
    {
        [DataMember]
        public int IdDireccion { get; set; }

        [DataMember]
        public int IdZona { get; set; }

        [DataMember]
        public string Direccion { get; set; }

        [DataMember]
        public bool Verificada { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }
    }
}