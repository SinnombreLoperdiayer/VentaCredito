using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Clase con el DataContract de los horarios de una ruta
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONHorarioRutaDC : DataContractBase
    {

        [DataMember]
        public int IdFrecuenciaRuta { get; set; }        

        [DataMember]
        public string IdDia { get; set; }

        [DataMember]
        public string HoraSalida { get; set; }

        [DataMember]
        public string HoraLlegada { get; set; }

        [DataMember]
        public string DescripcionDia { get; set; }

        [DataMember]
        public DateTime FechaActual { get; set; }

        [DataMember]
        public DateTime FechaSalida { get; set; }

    }
}
