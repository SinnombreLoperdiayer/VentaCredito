using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Clase que contiene la informacion del manifiesto nacional
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONManifiestoOperacionNalTerrestre : DataContractBase
  {
    [DataMember]
    public long IdManifiestoOperacionNacional { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Placa", Description = "ToolTipPlaca")]
    public string PlacaVehiculo { get; set; }

    [DataMember]
    public int IdVehiculo { get; set; }

    [DataMember]
    public string CedulaConductor { get; set; }

    [DataMember]
    public long IdConductor { get; set; }

    [DataMember]
    public string NombreConductor { get; set; }
  }
}