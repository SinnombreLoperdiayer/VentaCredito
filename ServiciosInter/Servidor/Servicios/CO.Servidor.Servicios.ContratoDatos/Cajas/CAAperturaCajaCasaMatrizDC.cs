using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAAperturaCajaCasaMatrizDC : DataContractBase
  {
    /// <summary>
    /// Es la Llave de la tbl con AperturaCaja
    /// </summary>
    /// <value>
    /// The id apertura caja matriz.
    /// </value>
    [DataMember]
    public long IdAperturaCajaMatriz { get; set; }

    /// <summary>
    /// Es el id de la caja
    /// </summary>
    [DataMember]
    public int IdCaja { get; set; }

    /// <summary>
    /// muestra si la caja esta abierta o cerrada
    /// </summary>
    [DataMember]
    public bool EstaAbierta { get; set; }

    /// <summary>
    /// Es la base inicial del Banco - Operacion nacional - Casa Matriz
    /// </summary>
    [DataMember]
    public decimal BaseInicial { get; set; }

    /// <summary>
    /// fecha en la que se realizo la Apertura
    /// </summary>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que realizo la Apertura de la Caja
    /// </summary>
    [DataMember]
    public string UsuarioApertura { get; set; }

    /// <summary>
    /// Codigo del usuario  ue realizo la apertura
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    /// <summary>
    /// Es el tipo de Apertura de la Caja
    /// OPN - CAM - BAN
    /// </summary>
    [DataMember]
    public string TipoApertura { get; set; }

    /// <summary>
    /// Es el id de la Casa Matriz
    /// </summary>
    [DataMember]
    public short IdCasaMatriz { get; set; }

    /// <summary>
    /// Nombre de la Empresa
    /// </summary>
    [DataMember]
    public string NombreEmpresa { get; set; }

    /// <summary>
    /// Es el nit de la Empresa
    /// </summary>
    [DataMember]
    public string NitEmpresa { get; set; }

    /// <summary>
    /// Es le centro de costos
    /// </summary>
    [DataMember]
    public string CentroDeCostos { get; set; }

    /// <summary>
    /// Es el estado de la Csa Matriz
    /// </summary>
    [DataMember]
    public string Estado { get; set; }
  }
}