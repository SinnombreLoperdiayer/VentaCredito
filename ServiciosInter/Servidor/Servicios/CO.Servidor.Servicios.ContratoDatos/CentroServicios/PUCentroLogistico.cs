using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
  /// <summary>
  /// Clase que contiene la informacion de los centros logisticos (COL)
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PUCentroLogistico : DataContractBase
  {
    [DataMember]
    public PUCentroServiciosDC CentroServicios { get; set; }

    [DataMember]
    public long IdRegionalAdm { get; set; }

    [DataMember]
    public List<PUCentroServiciosDC> PuntosAgencias { get; set; }

    [DataMember]
    public string Nombre { get; set; }
  }
}