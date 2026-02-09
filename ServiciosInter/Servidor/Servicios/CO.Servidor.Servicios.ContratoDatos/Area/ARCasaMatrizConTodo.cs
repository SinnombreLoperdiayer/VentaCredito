using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Area
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ARCasaMatrizConTodo : DataContractBase
  {
    [DataMember]
    public ARCasaMatrizDC CasaMatriz { get; set; }

    [DataMember]
    public IList<ARMacroprocesoDC> MacroProcesos { get; set; }

    [DataMember]
    public IList<ARGestionDC> Gestiones { get; set; }

    [DataMember]
    public IList<ARProcesoDC> Procesos { get; set; }
  }
}