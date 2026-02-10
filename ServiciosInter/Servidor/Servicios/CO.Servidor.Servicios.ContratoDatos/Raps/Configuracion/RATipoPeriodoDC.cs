using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RATipoPeriodoDC
    {
        [DataMember]
        public int IdTipoPeriodo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public int Periodos { get; set; }

        [DataMember]
        public bool Estado { get; set; }
    } 
}
