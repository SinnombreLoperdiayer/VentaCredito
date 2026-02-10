using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Clientes;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCNovedadCambioRemitenteDC : ADNovedadGuiaDC
  {
    [DataMember]
    public CLClienteContadoDC DestinatarioAnterior { get; set; }

    [DataMember]
    public CLClienteContadoDC DestinatarioNuevo { get; set; }

    [DataMember]
    public CLClienteContadoDC RemitenteAnterior { get; set; }

    [DataMember]
    public CLClienteContadoDC RemitenteNuevo { get; set; }
  }
}