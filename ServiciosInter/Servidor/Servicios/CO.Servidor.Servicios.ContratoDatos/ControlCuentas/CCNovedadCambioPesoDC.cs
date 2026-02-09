using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
     [DataContract(Namespace = "http://contrologis.com")]
    public class CCNovedadCambioPesoDC : ADNovedadGuiaDC
    {
         [DataMember]
         public decimal NuevoPeso { get; set; }
         [DataMember]
         public decimal PesoActual { get; set; }

    }
}
