using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  /// <summary>
  /// Clase que contiene la informacion de las comisiones ganadas por los servicios
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CMServiciosCentroServicios : DataContractBase
  {
    public event EventHandler OnUnidadNegocioCambio;

    public CMServiciosCentroServicios()
    {
      if (Servicios == null)
        Servicios = new TAServicioDC();
      Servicios.OnUnidadNegocioCambio += (o, e) =>
        {
          if (OnUnidadNegocioCambio != null)
            OnUnidadNegocioCambio(o, null);
        };
    }

    [DataMember]
    public long? IdCentroServiciosServicio { get; set; }

    [DataMember]
    public long IdCentroServicios { get; set; }

    [DataMember]
    public TAServicioDC Servicios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicioVenta", Description = "ToolTipFechaInicioVenta")]
    public DateTime FechaInicioVenta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "TooltipEstado")]
    public bool Estado { get; set; }

    [DataMember]
    public IList<PUHorariosServiciosCentroServicios> HorariosServicios { get; set; }

    private ObservableCollection<CMCentroServiciosServicioDescuento> descuentosServicios;

    [DataMember]
    public ObservableCollection<CMCentroServiciosServicioDescuento> DescuentosServicios
    {
      get { return descuentosServicios; }
      set
      {
        descuentosServicios = value;
      }
    }

    [DataMember]
    public ObservableCollection<CMCentroServicioServicioComi> ComisionesServicios { get; set; }

    [IgnoreDataMember]
    public ObservableCollection<CMCentroServicioServicioComi> ComisionesServiciosBorrados { get; set; }

    [IgnoreDataMember]
    public ObservableCollection<CMCentroServiciosServicioDescuento> DescuentosServiciosBorrados { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DiasServicio", Description = "ToolTipDiasServicio")]
    public string DiasServicio { get; set; }

    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }

    [Filtrable("SER_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "Nombre", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 25)]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre")]
    public string NombreServicio { get; set; }

    //[DataMember]
    //[Filtrable("SER_Nombre", typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), "UnidadNegocio", COEnumTipoControlFiltro.ComboBox)]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UnidadNegocio", Description = "TooltipUnidadNegocio")]
    //public string UnidadNegocio { get; set; }
  }
}