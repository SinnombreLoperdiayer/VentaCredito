using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
     [DataContract(Namespace = "http://contrologis.com")]
    public class    OUAsignacionRecogidaMensajeroDC
    {

         [DataMember]
         public long IdMensajero { get; set; }

         [DataMember]
         public long IdSolicitudRecogida { get; set; }

         [DataMember]
         public long IdAsignacion { get; set; }

         //public OUMensajeroDC MensajeroAsignacion { get; set; }
         //[DataMember]
         //public OURecogidasDC Recogida { get; set; }

          [DataMember]
         public string  NombreApellidoMensajero {get;set;}

          [DataMember]
         public string IdentificacionMensajero { get; set; }

         [DataMember]
          public string NumeroDocumentoCliente { get; set; }
         [DataMember]
         public string DireccionRecogida { get; set; }
         [DataMember]
         public string IdLocalidadRecogida { get; set; }

         [DataMember]
         public string Telefono { get; set; }

         [DataMember]
         public string PlacaVehiculo { get; set; }

         [DataMember]
         public string TipoVehiculo { get; set; }
         [DataMember]
         public string Usuario { get; set; }

    }
}
