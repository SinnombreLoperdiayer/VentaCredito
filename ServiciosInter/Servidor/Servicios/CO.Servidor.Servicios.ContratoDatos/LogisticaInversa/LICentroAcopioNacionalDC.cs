using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Informacion del Ingreso al Centro de Acopio Nacional
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIIngresoCentroAcopioNacionalDC : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public DateTime FechaDeIngreso { get; set; }

        [DataMember]
        public string CiudadDeIngreso { get; set; }

        [DataMember]
        public string Ruta { get; set; }

        [DataMember]
        public string PlacaVehiculo { get; set; }

        [DataMember]
        public string NombreDelConductor { get; set; }

        [DataMember]
        public string Novedad { get; set; }

    }
}
