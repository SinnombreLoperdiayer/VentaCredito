using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  /// <summary>
  /// Clase que contiene la información de anulaciones
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCAnulacionGuiaMensajeriaDC : ADNovedadGuiaDC
  {
    [DataMember]
    public long NumeroGuia { get; set; }

    [DataMember]
    public ADMotivoAnulacionDC MotivoAnulacion { get; set; }

    [DataMember]
    public ADTrazaGuia TrazaGuia { get; set; }

    [DataMember]
    public int IdCaja { get; set; }
  }
}