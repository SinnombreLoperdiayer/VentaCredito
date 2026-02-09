using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUMunicipiosSinAlCobro : DataContractBase
  {
    [DataMember]
    [Filtrable("LOC_Nombre", "Nombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 15)]
    public PALocalidadDC Municipio { get; set; }
  }
}
