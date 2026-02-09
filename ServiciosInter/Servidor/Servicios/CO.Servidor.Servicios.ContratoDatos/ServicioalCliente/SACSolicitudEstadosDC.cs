using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ServicioalCliente
{
     [DataContract(Namespace = "http://contrologis.com")]
   public class SACSolicitudEstadosDC: DataContractBase
   {
       [DataMember]
       public long IdSolicitud{ get; set; }

       [DataMember]
       public SACEstadosSolicitudDC Estado { get; set; }

       [DataMember]
       public EnumEstadoRegistro EstadoRegistro { get; set; }

   }
}
