using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    /// <summary>
    /// Clase que contiene las Territoriales
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUTerritorialDC : DataContractBase
    {
        [DataMember]
        public int IdTerritorial { get; set; }

        [DataMember]
        public string NombreTerritorial { get; set; }
    }
}
