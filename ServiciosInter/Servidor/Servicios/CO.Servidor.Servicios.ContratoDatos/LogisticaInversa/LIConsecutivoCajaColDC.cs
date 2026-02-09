using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIConsecutivoCajaColDC : DataContractBase
  {
    [DataMember]
    public long IdCentroLogistico { get; set; }

    [DataMember]
    public short IdConsecutivo { get; set; }
  }
}