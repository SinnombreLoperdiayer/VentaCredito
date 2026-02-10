using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class PAConsecutivoIngresoEgresoDC : DataContractBase
  {
    /// <summary>
    /// Es el consecutivo de Ingreso de dinero para el formulario
    /// de translado entre cajas
    /// </summary>
    [DataMember]
    public long ConsecutivoIngreso { get; set; }

    /// Es el consecutivo de Egreso de dinero para el formulario
    /// de translado entre cajas
    [DataMember]
    public long ConsecutivoEgreso { get; set; }
  }
}