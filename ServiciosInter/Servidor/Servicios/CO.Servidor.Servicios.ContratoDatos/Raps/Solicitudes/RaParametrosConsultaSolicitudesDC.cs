using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]   
    public class RaParametrosConsultaSolicitudesDC
    {
        [DataMember]
        public int Pagina { get; set; }

        [DataMember]
        public int RegistrosXPagina { get; set; }
        
        [DataMember]
        public string OrdenarPor { get; set; }

        [DataMember]
        public int IdEstado { get; set; }

        [DataMember]
        public string Filtro { get; set; }
        
        [DataMember]
        public int ValorFiltro { get; set; }

    }
}
