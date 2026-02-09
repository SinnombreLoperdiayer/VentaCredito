using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CACierreAutomaticoDC : DataContractBase
  {
    /// <summary>
    /// es el id de la aperturaque se utiliza para el mismo Cierre
    /// </summary>
    [DataMember]
    public long IdAperturaCaja { get; set; }

    /// <summary>
    /// Es el id de la Caja
    /// </summary>
    [DataMember]
    public int IdCaja { get; set; }

    /// <summary>
    /// es la base inicial de la caja
    /// </summary>
    [DataMember]
    public decimal BaseInicial { get; set; }

    /// <summary>
    /// Codigo del Usuario con el que se Abrio la Caja
    /// </summary>
    [DataMember]
    public long idCodigoUsuario { get; set; }

    /// <summary>
    /// es el id del centro de servicio al que corresponde la caja
    /// </summary>
    [DataMember]
    public long IdCentroServicios { get; set; }

    /// <summary>
    /// Nombre del Centro de Servicio
    /// </summary>
    [DataMember]
    public string NombreCentroServicio { get; set; }

    /// <summary>
    /// Fecha en la que se realizo la
    /// apertura de la Caja
    /// </summary>
    [DataMember]
    public DateTime FechaApertura { get; set; }

    /// <summary>
    /// Es el tipo del centro de servicio PTO-AGE-COL-RAC
    /// </summary>
    [DataMember]
    public string TipoCentroSvc { get; set; }
  }
}