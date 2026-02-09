using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase con la informacion del reporte de
  /// el dinero recibido por la Empresa de la agencia
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAReporteDineroAgenciaDC : DataContractBase
  {
    [DataMember]
    public int IdRecoleccion { get; set; }

    [DataMember]
    public decimal ValorReportado { get; set; }

    [DataMember]
    public bool RegistroManual { get; set; }

    [DataMember]
    public short TipoObservacion { get; set; }

    [DataMember]
    public string Observacion { get; set; }

    [DataMember]
    public string UsuarioRegistro { get; set; }
  }
}