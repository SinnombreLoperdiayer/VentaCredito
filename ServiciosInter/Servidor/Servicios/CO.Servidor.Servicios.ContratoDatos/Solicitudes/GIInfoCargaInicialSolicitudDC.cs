using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;

namespace CO.Servidor.Servicios.ContratoDatos.Solicitudes
{
  /// <summary>
  /// Clase que contiene las clases de la Carga inicial del VM de la Solicitud
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class GIInfoCargaInicialSolicitudDC : DataContractBase
  {
    /// <summary>
    /// es la info de los estado de los estados
    /// de una solicitud
    /// </summary>
    [DataMember]
    public List<SEEstadoUsuario> LstEstados { get; set; }

    /// <summary>
    /// Es la lista de los tipos de Identificacion
    /// de venta de un giro
    /// </summary>
    [DataMember]
    public IList<PATipoIdentificacion> LstTipoIdentificacion { get; set; }

    /// <summary>
    /// Es la lista de los tipos de Identificacion
    /// con los que se reclama un giro
    /// </summary>
    [DataMember]
    public IList<PATipoIdentificacion> LstTipoIdentificacionReclamaGiro { get; set; }
  }
}