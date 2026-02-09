using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Clientes;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Clase que contiene la informacion a ser preimpresa para los Suministros Manuales
  /// de una sucursal (Cliente Credito)
  /// </summary>  
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUImpresionSumSucursalDC : SUImpresionSumPadreDC
  {

    /// <summary>
    /// Informacion de la sucursal aprovisionado
    /// </summary>
    [DataMember]
    public CLSucursalDC Sucursal { get; set; }

    /// <summary>
    /// Almacena la configuración de referencia para uso de las guías de un cliente según su condición de uso:
    /// Origen Cerrado-Destino Abierto: OC-DA
    /// Origen Cerrado-Destino Cerrado: OC-DC
    /// Origen Abierto-Destino Cerrado: OA-DC
    /// </summary>
    [DataMember]
    public CLReferenciaUsoGuiaDC ReferenciaUsoGuia { get; set; }

    /// <summary>
    /// Rango aprovisionado de una sucursal
    /// </summary>
    [DataMember]
    public SURango Rango { get; set; }
  }
}
