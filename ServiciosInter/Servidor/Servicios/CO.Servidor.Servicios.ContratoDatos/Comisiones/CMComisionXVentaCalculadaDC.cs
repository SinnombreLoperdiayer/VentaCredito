using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Esta clase contiene la comisión calculada de una agencia/punto,
  /// de una agencia o racol responsable y de la empresa a partir
  /// de un servicio y una tarifa cobrada
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMComisionXVentaCalculadaDC : DataContractBase
  {
    [DataMember]
    public long NumeroOperacion { get; set; }

    [DataMember]
    public decimal BaseComision { get; set; }

    [DataMember]
    public int IdServicio { get; set; }

    [DataMember]
    public long IdCentroServicioVenta { get; set; }

    [DataMember]
    public string NombreCentroServicioVenta { get; set; }

    [DataMember]
    public long IdCentroServicioResponsable { get; set; }

    [DataMember]
    public string NombreCentroServicioResponsable { get; set; }

    [DataMember]
    public decimal TotalComisionCentroServicioVenta { get; set; }

    [DataMember]
    public decimal ValorFijoComisionCentroServicioVenta { get; set; }

    [DataMember]
    public decimal PorcComisionCentroServicioVenta { get; set; }

    [DataMember]
    public decimal TotalComisionCentroServicioResponsable { get; set; }

    [DataMember]
    public decimal ValorFijoComisionCentroServicioResponsable { get; set; }

    [DataMember]
    public decimal PorcComisionCentroServicioResponsable { get; set; }

    [DataMember]
    public decimal TotalComisionEmpresa { get; set; }

    public CMEnumTipoComision TipoComision { get; set; }

    /// <summary>
    /// Fecha de la producción donde se incluye este registro de venta
    /// </summary>
    /// <value>
    /// The fecha produccion.
    /// </value>
    [DataMember]
    public DateTime FechaProduccion { get; set; }

    /// <summary>
    /// Fecha de producción donde se inlcuye la comisión del reponsable
    /// </summary>
    /// <value>
    /// The fecha produccion responsable.
    /// </value>
    [DataMember]
    public DateTime FechaProduccionResponsable { get; set; }

    /// <summary>
    /// Es la fecha de Grabacion del Sistema
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Es el usuario que registro la Operacion
    /// </summary>
    /// <value>
    /// The creado por.
    /// </value>
    [DataMember]
    public string CreadoPor { get; set; }

    /// <summary>
    /// Indicador para registro valida, indica si la comisión liquidada es valida para incluirla en una producción, util por ejemplo cuando se liquida una comisión por un 
    /// AlCobro pero éste todavia no se ha descargado
    /// </summary>
    private bool esRegistroValido = true;

    /// <summary>
    /// Indicador para registro valida, indica si la comisión liquidada es valida para incluirla en una producción, util por ejemplo cuando se liquida una comisión por un 
    /// AlCobro pero éste todavia no se ha descargado
    /// </summary>
    public bool EsRegistroValido
    {
      get { return esRegistroValido; }
      set { esRegistroValido = value; }
    }
    
  }
}