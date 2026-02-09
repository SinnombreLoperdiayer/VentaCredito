using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCEncabezadoArchivoAjusteGuiaDC : DataContractBase
    {
      [DataMember]
      public string NombreArchivo { get; set; }

      [DataMember]
      public int TotalRegistros { get; set; }

      [DataMember]
      public string Guid { get; set; }

      [DataMember]
      public long Id { get; set; }

      [DataMember]
      public string Usuario { get; set; }

      [DataMember]
      public short Estado { get; set; }

      [DataMember]
      public string DescripcionEstado { get; set; }

      [DataMember]
      public DateTime Fecha { get; set; }
   }
}
