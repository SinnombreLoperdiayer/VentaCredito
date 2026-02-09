using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  /// <summary>
  /// Clase que contiene el precio por cada trayecto y subtrayecto
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class TAPrecioTrayectoRangoDC : DataContractBase
  {
    public event EventHandler OnCambioPorcentaje;

    [DataMember]
    public long IdPrecioTrayectoRango { get; set; }

    [DataMember]
    [CamposOrdenamiento("PPR_Inicial")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoPesoInicial")]
    public decimal PesoInicial { get; set; }

    [DataMember]
    [CamposOrdenamiento("PPR_Final")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RangoPesoFinal")]
    public decimal PesoFinal { get; set; }

    [DataMember]
    [CamposOrdenamiento("PPR_Valor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorBaseKiloAdicional")]
    public decimal KiloAdicional { get; set; }

    [DataMember]
    [CamposOrdenamiento("PPR_Valor")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
    public decimal Valor { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorPropuestoKiloAdicional")]
    public decimal ValorPropuesto { get; set; }

    private decimal porcentaje;

    [DataMember]
    [CamposOrdenamiento("PPR_Porcentaje")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Porcentaje")]
    public decimal Porcentaje
    {
      get { return porcentaje; }
      set
      {
        porcentaje = value;
        if (Porcentaje != 0)
          if (OnCambioPorcentaje != null)
            OnCambioPorcentaje(null, null);
      }
    }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}