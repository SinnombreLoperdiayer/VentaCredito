using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class CLTipoClienteCreditoDC : DataContractBase
    {
      [DataMember]
      public int IdTipoClienteCredito { get; set; }

      [DataMember]
      public string Descripcion {get; set;}

      [DataMember]
      public DateTime FechaGrabacion {get; set;}

      [DataMember]
      public string CreadoPor { get; set; }
    }
}
