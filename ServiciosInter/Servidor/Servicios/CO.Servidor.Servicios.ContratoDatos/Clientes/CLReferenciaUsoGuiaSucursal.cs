using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Clientes
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CLReferenciaUsoGuiaSucursal
  {
    /// <summary>
    /// Condición de uso de la guía, los valores se obtienen de ConstantesComun.CONDICION_USO_ORIGEN_CERRADO_DESTINO_ABIERTO, CONDICION_USO_ORIGEN_CERRADO_DESTINO_CERRADO o CONDICION_USO_ORIGEN_ABIERTO_DESTINO_CERRADO
    /// </summary>
    [DataMember]
    public string CondicionUso { get; set; }

    [DataMember]
    public int SucursalId { get; set; }

    [DataMember]
    public string PaisDestino { get; set; }

    [DataMember]
    public string CiudadDestino { get; set; }

    [DataMember]
    public string TipoIdentificacion { get; set; }

    [DataMember]
    public string Identificacion { get; set; }

    [DataMember]
    public string TelefonoDestino { get; set; }

    [DataMember]
    public string NombreDestinatario { get; set; }

    [DataMember]
    public string DireccionDestinatario { get; set; }
  }
}