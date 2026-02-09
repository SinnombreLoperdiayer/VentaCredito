using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene las listas utilizadas en la administracion de los centros de servicio
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUListasCentrosServicio : DataContractBase
  {
    [DataMember]
    public IList<PUTipoAgencia> TiposAgencia { get; set; }

    [DataMember]
    public IList<PUTipoPropiedad> TiposPropiedad { get; set; }

    [DataMember]
    public IList<PUCentroServicioApoyo> CentrosServiciosApoyo { get; set; }

    [DataMember]
    public IList<PUEstadoDC> Estados { get; set; }

    [DataMember]
    public IList<PUTipoCentroServicio> TiposCentroServicio { get; set; }

    [DataMember]
    public IList<PUClasificadorCanalVenta> ClasificadoresCanalVenta { get; set; }

  }
}