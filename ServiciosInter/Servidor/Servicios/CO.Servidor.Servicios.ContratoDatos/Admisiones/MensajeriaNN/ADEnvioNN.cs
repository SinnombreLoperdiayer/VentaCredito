using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.MensajeriaNN
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADEnvioNN
    {
        [DataMember]
        public ADGuia EnvioNN { get; set; }
        [DataMember]
        public long NumeroMensajeriaNN { get; set; }
        [DataMember]
        public string IdCiudadCaptura { get; set; }
        [DataMember]
        public string CiudadCaptura { get; set; }
        [DataMember]
        public long IdCentroServicioCaptura { get; set; }
        [DataMember]
        public string CentroServicioCaptura { get; set; }
        [DataMember]
        public DateTime FechaCaptura { get; set; }
        [DataMember]
        public int IdOperativoOrigen { get; set; }
        [DataMember]
        public string OperativoOrigen { get; set; }
        [DataMember]
        public int IdEstado { get; set; }
        [DataMember]
        public List<string> Imagenes { get; set; }
        [DataMember]
        public bool Estado { get; set; }
        [DataMember]
        public string DescripcionEmpaque { get; set; }
        [DataMember]
        public ClasificacionEnvioNN Clasificacion { get; set; }
    }

   
   

}
