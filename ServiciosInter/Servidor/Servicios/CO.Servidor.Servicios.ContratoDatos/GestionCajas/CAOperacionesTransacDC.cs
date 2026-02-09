using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Servicios.ContratoDatos;

using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase contiene la info de las
  /// transacciones entre las cajas
  /// Consignacion - Retiro - Mov efectivo
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAOperacionesTransacDC : DataContractBase
  {
    /// <summary>
    /// Es el centro de servicio Asociado a la
    /// transaccion
    /// </summary>
    /// <value>
    /// The centro servicio.
    /// </value>
   
    private PUCentroServiciosDC centroServicio;
    
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicio", Description = "TooltipCentroServicio")]
    public PUCentroServiciosDC CentroServicio
    {
      get { return centroServicio; }
      set { centroServicio = value; OnPropertyChanged("CentroServicio"); }
    }

    /// <summary>
    /// Es el banco sobre el cual se realiza la transaccion
    /// </summary>
    /// <value>
    /// The banco.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Banco", Description = "TooltipBanco")]
    public PABanco Banco { get; set; }

    /// <summary>
    /// Es la cta del banco sobre la que se realiza la transaccion
    /// </summary>
    /// <value>
    /// The cuenta banco.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroCuenta", Description = "TooltipCuentaBancaria")]
    public ARCuentaCasaMatrizDC CuentaBanco { get; set; }

    /// <summary>
    /// es donde se registra el numero de la consignación ó
    /// numero de cheque de la transaccion realizada
    /// </summary>
    /// <value>
    /// The numero transaccion.
    /// </value>
    [DataMember]
    [Required(ErrorMessage="Debe ingresar el número de la consignación")]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroOperacion", Description = "ToolTipNumeroOperacion")]
    public string NumeroTransaccion { get; set; }

    /// <summary>
    /// Es la fecah en la que se realiza la transaccion
    /// </summary>
    /// <value>
    /// The fecha transaccion.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaTransaccion")]
    public DateTime FechaTransaccion { get; set; }

    /// <summary>
    /// Es el valor de la transaccion
    /// </summary>
    /// <value>
    /// The valor transaccion.
    /// </value>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido", AllowEmptyStrings = false)]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal ValorTransaccion { get; set; }

    /// <summary>
    /// Es la observacion referente a la
    /// transaccion realizada.
    /// </summary>
    /// <value>
    /// The observacion.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones")]
    public string Observacion { get; set; }

    /// <summary>
    /// Es el concepto.
    /// </summary>
    /// <value>
    /// The concepto caja.
    /// </value>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Concepto")]
    public CAConceptoCajaDC ConceptoCaja { get; set; }
  }
}