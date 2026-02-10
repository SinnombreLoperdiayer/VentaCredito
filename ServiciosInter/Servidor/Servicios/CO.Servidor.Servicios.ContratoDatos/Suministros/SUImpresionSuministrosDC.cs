using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Contiene la informacion de la impresion de suministros
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUImpresionSuministrosDC
  {

    /// <summary>
    /// Contiene la informacion a ser preimpresa para los Suministros de un Mensajero  
    /// </summary>
    [DataMember]
    public List<SUImpresionSumMensajeroDC> ImpresionSumMensajero { get; set; }


    /// <summary>
    /// Consulta los suministros de las sucursales
    /// </summary>
    [DataMember]
    public List<SUImpresionSumSucursalDC> ImpresionSumSucursal { get; set; }


    /// <summary>
    /// Clase que contiene la informacion a ser preimpresa para los Suministros Manuales
    /// de un Centro de Servicio
    /// </summary>
    [DataMember]
    public List<SUImpresionSumCentroServicioDC> ImpresionSumCentroServicio { get; set; }

     /// <summary>
    /// Consulta los suministros de las sucursales
    /// </summary>
    [DataMember]
    public List<SUImpresionSumGestionDC> ImpresionSumGestion { get; set; }

  }
}
