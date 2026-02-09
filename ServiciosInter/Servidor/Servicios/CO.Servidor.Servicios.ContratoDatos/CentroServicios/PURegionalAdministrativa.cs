using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    /// <summary>
    /// Clase que contiene la informacion de las regionales administrativas (RACOL)
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class PURegionalAdministrativa : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "RegionalAdministrativa", Description = "TooltipRegionalAdministrativa")]
        public long IdRegionalAdmin { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "RegionalAdministrativa", Description = "TooltipRegionalAdministrativa")]
        public string Descripcion { get; set; }

        [DataMember]
        public PUCentroServiciosDC CentroServicios { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CentroCostosRegional", Description = "CentroCostosRegional")]
        public string IdCentroCosto { get; set; }

        [DataMember]
        public short IdCasaMatriz { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [IgnoreDataMember]
        public ObservableCollection<PUCentroServiciosDC> CentrosServicios { get; set; }
    }
}