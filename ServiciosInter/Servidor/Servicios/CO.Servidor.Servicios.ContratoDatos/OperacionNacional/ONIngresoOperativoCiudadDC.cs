using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion del ingreso a COL por Ciudad
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONIngresoOperativoCiudadDC : DataContractBase
  {
    /// <summary>
    /// Ruta de la operacion
    /// </summary>
    [DataMember]
    public long IdIngresoOperativo { get; set; }

    ///// <summary>
    ///// Ruta de la operacion
    ///// </summary>
    //[DataMember]
    //public RURutaDC Ruta { get; set; }

    /// <summary>
    /// Vehiculo del descargue
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Placa", Description = "ToolTipPlaca")]
    public POVehiculo Vehiculo { get; set; }

    /// <summary>
    /// Informacion del conductor del vehiculo del descargue
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Conductores", Description = "Conductores")]
    public POConductores Conductores { get; set; }

    /// <summary>
    /// Fecha del descargue de la operacion por ruta
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue", Description = "FechaDescargue")]
    public DateTime FechaDescargue { get; set; }

    /// <summary>
    /// Ciudad donde se esta haciendo el descargue de la operacion
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
    public PUCentroServiciosDC CiudadDescargue { get; set; }

    /// <summary>
    /// Ciudad donde se esta haciendo el ingreso de la operacion
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
    public PALocalidadDC CiudadIngreso { get; set; }

    /// <summary>
    /// Agencia donde se esta haciendo el ingreso
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Agencia", Description = "Agencia")]
    public long IdAgencia { get; set; }

    /// <summary>
    /// Retorna o asigna los consolidados del manifiesto
    /// </summary>
    [DataMember]
    public ONConsolidado Consolidado { get; set; }

    ///// <summary>
    ///// Manifiestos del ingreso
    ///// </summary>
    //[DataMember]
    //public List<ONManifiestoOperacionNacional> Manifiestos { get; set; }

    ///// <summary>
    ///// Manifiestos del ingreso
    ///// </summary>
    [DataMember]
    public ONManifiestoOperacionNacional ManifiestoIngreso { get; set; }

    /// <summary>
    /// Retorna o asigna el envio para el ingreso a centro logistico
    /// </summary>
    [DataMember]
    public ONEnviosDescargueRutaDC EnvioIngreso { get; set; }
  }
}