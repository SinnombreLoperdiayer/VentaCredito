using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAProcedimientoDC
    {
        [DataMember]
        public string IdProcedimiento { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public int IdProceso { get; set; }
    } 

}
