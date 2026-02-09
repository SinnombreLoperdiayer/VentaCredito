using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCNovedadCambioDestinoDC : ADNovedadGuiaDC
  {
    /// <summary>
    /// Nueva ciudad destino
    /// </summary>
    [DataMember]
    public PALocalidadDC NuevaLocalidadDestino { get; set; }

    /// <summary>
    /// Ciudad actual donde se encuentra el envío
    /// </summary>
    [DataMember]
    public PALocalidadDC LocalidadActual { get; set; }
  }
}