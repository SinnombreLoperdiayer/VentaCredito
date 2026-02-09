using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUCentroServicioReporte : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCentroServicio", Description = "TooltipIdCentroServicio")]
        [Filtrable("CRD_IdCentroServiciosQueReporta", "Id Centro Servicio:", COEnumTipoControlFiltro.TextBox, FormatoRegex = ("[0-9]"), MaximaLongitud = 100)]
        [CamposOrdenamiento("CRD_IdCentroServiciosQueReporta")]
        public long IdCentroServicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "ToolTipNombreCentroServicio")]
        [Filtrable("CES_Nombre", "Nombre Centro Servicio:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 250)]
        [CamposOrdenamiento("CES_Nombre")]
        public string Nombre { get; set; }

        [IgnoreDataMember]
        public string NombreId
        {
            get
            {
                return string.Join(" - ", IdCentroServicio.ToString(), Nombre);
            }
            set { }
        }
    }
}
