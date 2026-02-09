using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIIngresoCentroAcopioUrbanoDC: DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long? NumeroPlanilla { get; set; }

        [DataMember]
        public string EstadoEmpaque { get; set; }

        [DataMember]
        public long IngresoPlanilla { get; set; }

        [DataMember]
        public string NombreCiudad { get; set; }

        [DataMember]
        public string CedulaMensajero { get; set; }

        [DataMember]
        public string NombreMensajero { get; set; }

        [DataMember]
        public DateTime FechaIngreso { get; set; }

        [DataMember]
        public string UsuarioIngreso { get; set; }

    }
}
